using Microsoft.VisualBasic.FileIO;

namespace Predictor;

internal class Program
{
    // --- Main methods called
    static void Main(string[] args)
    {
        /*
        List<int> Grid = GenerateGrid();

        string generatedGrid = "";
        int index = 0;
        foreach (int number in Grid) {
            if(index == 0) generatedGrid = generatedGrid + number;
            else if (index > 0 && index < 5) generatedGrid = generatedGrid +  " - " + number;
            else generatedGrid = generatedGrid + " - Star - " + number;

            index++;
        }
        Console.WriteLine(generatedGrid);
        */

        Console.WriteLine("---> Load file...");

        List<string> tirages = OpenDataFile();
        Console.WriteLine(tirages.Count() + "\n");

        Console.WriteLine("---> Statistics ...");
        Statistics(tirages);
    }


    // --- Program methods
    static public List<int> GenerateGrid()
    {
        List<int> Grid = new List<int>();

        // Balls
        while (Grid.Count < 5) {
            int newBall = RandomBall(false);
            if(!Grid.Contains(newBall)) Grid.Add(newBall);
        }

        // Star
        Grid.Add(RandomBall(true));

        return Grid;
    }

    static public List<string> OpenDataFile()
    {
        List<string> tirages = new List<string>();

        //var path = @"..\..\..\..\SIDE-PROJ-DATA\full_new_loto_21-02-2024.csv"; // Full current loto
        var path = @"..\..\..\..\SIDE-PROJ-DATA\full_new_loto_21-02-2024.csv"; // Full loto till 2017
        string[] latestAsString = System.IO.File.ReadAllLines(path);

        using (TextFieldParser parser = new TextFieldParser(GetTextReader(latestAsString)))
        {
            parser.SetDelimiters(new string[] { ";" });
            parser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            parser.ReadLine();

            string[] fieldColumns = parser.ReadFields();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                string hit = fields[10]; // should be like that : 9-22-33-42-45+3
                tirages.Add(hit);
            }
        }

        return tirages;
    }

    static void Statistics(List<string> tirageList)
    {
        // Dict for combinaisons
        Dictionary<int, int> countEachBalls = new Dictionary<int, int>();
        Dictionary<int, int> countEachStars = new Dictionary<int, int>();
        Dictionary<string, int> countBall_5_star = new Dictionary<string, int>();
        Dictionary<string, int> countBall_5 = new Dictionary<string, int>();
        Dictionary<string, int> countBall_4 = new Dictionary<string, int>();

        // --- Simple stats
        foreach (string tirage in tirageList) {
            List<int> allBallsNumber = new List<int>();
            List<int> allBallsNumberNoStar = new List<int>();

            // split the hit
            string formatedHit = tirage.Replace('+', '-');
            string[] hitNumbers = formatedHit.Split('-');

            // increment all tirage with star
            if (countBall_5_star.ContainsKey(formatedHit)) countBall_5_star[formatedHit]++;
            else countBall_5_star[formatedHit] = 1;

            // increment all tirage without star
            string comb5balls = hitNumbers[0]+ "-"+ hitNumbers[1] + "-" + hitNumbers[2] + "-" + hitNumbers[3] + "-" + hitNumbers[4];
            if (countBall_5.ContainsKey(comb5balls)) countBall_5[comb5balls]++;
            else countBall_5[comb5balls] = 1;

            // add count of each number
            int index = 0;
            foreach (string number in hitNumbers) {
                int ball = Int32.Parse(number);
                allBallsNumber.Add(ball);

                if (index < 5)
                {
                    allBallsNumberNoStar.Add(ball);
                    if (countEachBalls.ContainsKey(ball)) countEachBalls[ball]++;
                    else countEachBalls[ball] = 1;
                }
                else {
                    if (countEachStars.ContainsKey(ball)) countEachStars[ball]++;
                    else countEachStars[ball] = 1;
                }

                index++;
            }

            // increment all possible 4 number  combinaison (no star in count)
            // First ball
            List<int> newCombinaison = new List<int>();
            foreach (int ball1 in allBallsNumberNoStar)
            {
                newCombinaison.Add(ball1);

                // Second ball
                foreach (int ball2 in allBallsNumberNoStar)
                {
                    if (!newCombinaison.Contains(ball2)) {
                        newCombinaison.Add(ball2);

                        // Third ball
                        foreach (int ball3 in allBallsNumberNoStar)
                        {
                            if (!newCombinaison.Contains(ball3)) {
                                newCombinaison.Add(ball3);

                                // Fourth ball
                                foreach (int ball4 in allBallsNumberNoStar)
                                {
                                    if (!newCombinaison.Contains(ball4)) {
                                        newCombinaison.Add(ball4);
                                        newCombinaison.Sort();


                                        string combinaison = newCombinaison[0].ToString() + "-" + newCombinaison[1].ToString() + "-" + newCombinaison[2].ToString() + "-" + newCombinaison[3].ToString();

                                        if (countBall_4.ContainsKey(combinaison)) countBall_4[combinaison]++;
                                        else countBall_4[combinaison] = 1;
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        // --- Predictor
        // balls
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>> ballPredict = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>>();

        // stars
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>> starPredict = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>>();
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> tripleStarPredict = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

        // Running
        int star1 = 0;
        int star2 = 0;
        int star3 = 0;
        int star4 = 0;
        int star5 = 0;
        List<int> b1 = new List<int>();
        List<int> b2 = new List<int>();
        List<int> b3 = new List<int>();
        List<int> b4 = new List<int>();
        List<int> b5 = new List<int>();

        List<string> reversedTirageList = tirageList;
        reversedTirageList.Reverse();
        foreach (string tirage in reversedTirageList) {
            // split the hit : 0 - 1 - 2 - 3 - 4 - s5
            string formatedHit = tirage.Replace('+', '-');
            string[] hitNumbers = formatedHit.Split('-');

            // --- Balls
            List<int> onlyBalls = new List<int>
            {
                int.Parse(hitNumbers[0]),
                int.Parse(hitNumbers[1]),
                int.Parse(hitNumbers[2]),
                int.Parse(hitNumbers[3]),
                int.Parse(hitNumbers[4])
            };

            // Add first stars
            if (b1.Count() == 0) b1 = onlyBalls;
            else if (b2.Count() == 0) b2 = onlyBalls;
            else if (b3.Count() == 0) b3 = onlyBalls;
            else if (b4.Count() == 0) b4 = onlyBalls;
            else if (b5.Count() == 0) b5 = onlyBalls;
            // loop
            else
            {
                foreach (int saveB1 in b1) {
                    if (!ballPredict.ContainsKey(saveB1)) ballPredict[saveB1] = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();

                    foreach (int saveB2 in b2) {
                        if (!ballPredict[saveB1].ContainsKey(saveB2)) ballPredict[saveB1][saveB2] = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

                        foreach (int saveB3 in b3)
                        {
                            if (!ballPredict[saveB1][saveB2].ContainsKey(saveB3)) ballPredict[saveB1][saveB2][saveB3] = new Dictionary<int, Dictionary<int, int>>();

                            foreach (int saveB4 in b4)
                            {
                                if (!ballPredict[saveB1][saveB2][saveB3].ContainsKey(saveB4)) ballPredict[saveB1][saveB2][saveB3][saveB4] = new Dictionary<int, int>();

                                foreach (int saveB5 in b5)
                                {
                                    if (!ballPredict[saveB1][saveB2][saveB3][saveB4].ContainsKey(saveB5)) ballPredict[saveB1][saveB2][saveB3][saveB4][saveB5] = 1;
                                    else ballPredict[saveB1][saveB2][saveB3][saveB4][saveB5]++;
                                }
                            }
                        }
                    }
                }

                b1 = b2;
                b2 = b3;
                b3 = b4;
                b4 = b5;
                b5 = onlyBalls;
            }

            // --- Stars
            // Add first stars
            if (star1 == 0) star1 = int.Parse(hitNumbers[5]);
            else if (star2 == 0) star2 = int.Parse(hitNumbers[5]);
            else if (star3 == 0) star3 = int.Parse(hitNumbers[5]);
            else if (star4 == 0) star4 = int.Parse(hitNumbers[5]);
            else if (star5 == 0) star5 = int.Parse(hitNumbers[5]);
            // loop
            else {
                // save predict
                if (!starPredict.ContainsKey(star1)) starPredict[star1] = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();
                if (!starPredict[star1].ContainsKey(star2)) starPredict[star1][star2] = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
                if (!starPredict[star1][star2].ContainsKey(star3)) starPredict[star1][star2][star3] = new Dictionary<int, Dictionary<int, int>>();
                if (!starPredict[star1][star2][star3].ContainsKey(star4)) starPredict[star1][star2][star3][star4] = new Dictionary<int, int>();

                if (!starPredict[star1][star2][star3][star4].ContainsKey(star5)) starPredict[star1][star2][star3][star4][star5] = 1;
                else starPredict[star1][star2][star3][star4][star5]++; 
            
                star1 = star2;
                star2 = star3;
                star3 = star4;
                star4 = star5;
                star5 = Int32.Parse(hitNumbers[5]);
            }

        }

        // --- Print results
        string resultsFile = @"..\..\..\..\SIDE-PROJ-DATA\results.txt";

        // sort dictionnary
        Dictionary<int, int> sortedCountEachBalls = countEachBalls.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        Dictionary<int, int> sortedCountEachStars = countEachStars.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

        //Write to a file
        using (StreamWriter writer = new StreamWriter(resultsFile))
        {
            Console.WriteLine("---> Results");
            Console.WriteLine("- All tirages : " + countBall_5_star.Count());
            Console.WriteLine("- All tirages no star : " + countBall_5.Count());
            Console.WriteLine("- All combinaison : " + countBall_4.Count());
            writer.WriteLine("---> Results");
            writer.WriteLine("- All tirages : " + countBall_5_star.Count());
            writer.WriteLine("- All tirages no star : " + countBall_5.Count());
            writer.WriteLine("- All combinaison : " + countBall_4.Count());

            // --- Simple stats results
            // best/bad ball numbers
            foreach (int ball in sortedCountEachBalls.Keys)
            {
                if (sortedCountEachBalls[ball] > 120) Console.WriteLine("!! Best ball : " + ball.ToString() + " - nb fois : " + sortedCountEachBalls[ball].ToString());

                if (sortedCountEachBalls[ball] < 100) Console.WriteLine("- Bad ball : " + ball.ToString() + " - nb fois : " + sortedCountEachBalls[ball].ToString());
            }
            // best/bad star numbers
            foreach (int star in sortedCountEachStars.Keys)
            {
                if (sortedCountEachStars[star] > 120) Console.WriteLine("?? Best star : " + star.ToString() + " - nb fois : " + sortedCountEachStars[star].ToString());

                if (sortedCountEachStars[star] < 100) Console.WriteLine("- Bad star : " + star.ToString() + " - nb fois : " + sortedCountEachStars[star].ToString());
            }

            // best tirage
            foreach (string tirage in countBall_5.Keys)
            {
                if (countBall_5[tirage] > 1) Console.WriteLine("Tirage : " + tirage + " - nb fois : " + countBall_5[tirage].ToString());
            }
            // best 4 combinaison
            foreach (string tirage in countBall_4.Keys)
            {
                writer.WriteLine(tirage);
                if (countBall_4[tirage] > 2) Console.WriteLine("Combinaison : " + tirage + " - nb fois : " + countBall_4[tirage].ToString());
            }

            // --- Predictor results
            Console.WriteLine("\n---> Predictor");
            // strt prediction
            for (int i = 1; i < 11; i++)
            {
                if (starPredict.ContainsKey(i)) {
                    for (int j = 1; j < 11; j++)
                    {
                        if (starPredict[i].ContainsKey(j)) {
                            for (int k = 1; k < 11; k++)
                            {
                                if (starPredict[i][j].ContainsKey(k)) {
                                    for (int l = 1; l < 11; l++)
                                    {
                                        if (starPredict[i][j][k].ContainsKey(l)) {
                                            for (int m = 1; m < 11; m++)
                                            {
                                                if (starPredict[i][j][k][l].ContainsKey(m) && starPredict[i][j][k][l][m] > 1)
                                                {
                                                    Console.WriteLine("- Star Combinaison : " + i.ToString() + " -> " + j.ToString() + " -> " + k.ToString() + " -> " + l.ToString() + " best chance have " + m.ToString() + " : " + starPredict[i][j][k][l][m].ToString() + " chances");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // ball prediction
            for (int i = 1; i < 50; i++)
            {
                if (ballPredict.ContainsKey(i))
                {
                    for (int j = 1; j < 50; j++)
                    {
                        if (ballPredict[i].ContainsKey(j))
                        {
                            for (int k = 1; k < 50; k++)
                            {
                                if (ballPredict[i][j].ContainsKey(k))
                                {
                                    for (int l = 1; l < 50; l++)
                                    {
                                        if (ballPredict[i][j][k].ContainsKey(l))
                                        {
                                            for (int m = 1; m < 50; m++)
                                            {
                                                if (ballPredict[i][j][k][l].ContainsKey(m) && ballPredict[i][j][k][l][m] > 2)
                                                {
                                                    Console.WriteLine("- Ball Combinaison : " + i.ToString() + " -> " + j.ToString() + " -> " + k.ToString() + " -> " + l.ToString() + " best chance have " + m.ToString() + " : " + ballPredict[i][j][k][l][m].ToString() + " chances");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // --- Internal Methods
    public static TextReader GetTextReader(IEnumerable<string> lines)
    {
        // Transform IEnumerable<string> to TextReader
        return new StringReader(string.Join("\r\n", lines));
    }

    static private int RandomBall(bool isStar)
    {
        Random rnd = new Random();

        int number = 0;
        if (isStar) number = rnd.Next(1, 11);
        else number = rnd.Next(1, 50);

        return number;
    }
}