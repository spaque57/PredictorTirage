using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace Predictor;

internal class Program
{
    // --- Generic objects
    // Dict for combinaisons
    static Dictionary<int, int> countEachBalls = new Dictionary<int, int>();
    static Dictionary<int, int> countEachStars = new Dictionary<int, int>();
    static Dictionary<string, int> countBall_5_star = new Dictionary<string, int>();
    static Dictionary<string, int> countBall_5 = new Dictionary<string, int>();
    static Dictionary<string, int> countBall_4 = new Dictionary<string, int>();
    static Dictionary<string, int> countBall_3 = new Dictionary<string, int>();

    // balls
    static Dictionary<int, int> ballPredict1 = new Dictionary<int, int>();
    static Dictionary<int, Dictionary<int, int>> ballPredict2 = new Dictionary<int, Dictionary<int, int>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, int>>> ballPredict3 = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> ballPredict4 = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>> ballPredict = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>>();

    // stars
    static Dictionary<int, int> starPredict1 = new Dictionary<int, int>();
    static Dictionary<int, Dictionary<int, int>> starPredict2 = new Dictionary<int, Dictionary<int, int>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, int>>> starPredict3 = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> starPredict4 = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();
    static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>> starPredict = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>>();



    // --- Main methods called
    static void Main(string[] args)
    {
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
                                newCombinaison.Sort();

                                string combinaison3 = newCombinaison[0].ToString() + "-" + newCombinaison[1].ToString() + "-" + newCombinaison[2].ToString();

                                if (countBall_3.ContainsKey(combinaison3)) countBall_3[combinaison3]++;
                                else countBall_3[combinaison3] = 1;

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
                    // level 1
                    if (!ballPredict1.ContainsKey(saveB1)) ballPredict1[saveB1] = 1;
                    else ballPredict1[saveB1]++;
                    // level 2
                    if (!ballPredict2.ContainsKey(saveB1)) ballPredict2[saveB1] = new Dictionary<int, int>();
                    // level 3
                    if (!ballPredict3.ContainsKey(saveB1)) ballPredict3[saveB1] = new Dictionary<int, Dictionary<int, int>>();
                    // level 4
                    if (!ballPredict4.ContainsKey(saveB1)) ballPredict4[saveB1] = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

                    // level 5
                    if (!ballPredict.ContainsKey(saveB1)) ballPredict[saveB1] = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();
                    foreach (int saveB2 in b2) {
                        // level 2
                        if (!ballPredict2[saveB1].ContainsKey(saveB2)) ballPredict2[saveB1][saveB2] = 1;
                        else ballPredict2[saveB1][saveB2]++;
                        // level 3
                        if (!ballPredict3[saveB1].ContainsKey(saveB2)) ballPredict3[saveB1][saveB2] = new Dictionary<int, int>();
                        // level 4
                        if (!ballPredict4[saveB1].ContainsKey(saveB2)) ballPredict4[saveB1][saveB2] = new Dictionary<int, Dictionary<int, int>>();

                        // level 5
                        if (!ballPredict[saveB1].ContainsKey(saveB2)) ballPredict[saveB1][saveB2] = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
                        foreach (int saveB3 in b3)
                        {                        
                            // level 3
                            if (!ballPredict3[saveB1][saveB2].ContainsKey(saveB3)) ballPredict3[saveB1][saveB2][saveB3] = 1;
                            else ballPredict3[saveB1][saveB2][saveB3]++;
                            // level 4
                            if (!ballPredict4[saveB1][saveB2].ContainsKey(saveB3)) ballPredict4[saveB1][saveB2][saveB3] = new Dictionary<int, int>();

                            // level 5
                            if (!ballPredict[saveB1][saveB2].ContainsKey(saveB3)) ballPredict[saveB1][saveB2][saveB3] = new Dictionary<int, Dictionary<int, int>>();
                            foreach (int saveB4 in b4)
                            {
                                // level 4
                                if (!ballPredict4[saveB1][saveB2][saveB3].ContainsKey(saveB4)) ballPredict4[saveB1][saveB2][saveB3][saveB4] = 1;
                                else ballPredict4[saveB1][saveB2][saveB3][saveB4]++;

                                // level 5
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
                // LEVEL 1
                if (!starPredict1.ContainsKey(star1)) starPredict1[star1] = 1;
                else starPredict1[star1]++;
                // level 2
                if (!starPredict2.ContainsKey(star1)) starPredict2[star1] = new Dictionary<int, int>();
                // level 3
                if (!starPredict3.ContainsKey(star1)) starPredict3[star1] = new Dictionary<int, Dictionary<int, int>>();
                // level 4
                if (!starPredict4.ContainsKey(star1)) starPredict4[star1] = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
                // level 5
                if (!starPredict.ContainsKey(star1)) starPredict[star1] = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();

                // LEVEL 2
                if (!starPredict2[star1].ContainsKey(star2)) starPredict2[star1][star2] = 1;
                starPredict2[star1][star2]++;
                // level 3
                if (!starPredict3[star1].ContainsKey(star2)) starPredict3[star1][star2] = new Dictionary<int, int>();
                // level 4
                if (!starPredict4[star1].ContainsKey(star2)) starPredict4[star1][star2] = new Dictionary<int, Dictionary<int, int>>();
                // level 5
                if (!starPredict[star1].ContainsKey(star2)) starPredict[star1][star2] = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

                // LEVEL 3
                if (!starPredict3[star1][star2].ContainsKey(star3)) starPredict3[star1][star2][star3] = 1;
                else starPredict3[star1][star2][star3]++;
                // level 4
                if (!starPredict4[star1][star2].ContainsKey(star3)) starPredict4[star1][star2][star3] = new Dictionary<int, int>();
                // level 5
                if (!starPredict[star1][star2].ContainsKey(star3)) starPredict[star1][star2][star3] = new Dictionary<int, Dictionary<int, int>>();

                // LEVEL 4
                if (!starPredict4[star1][star2][star3].ContainsKey(star4)) starPredict4[star1][star2][star3][star4] = 1;
                else starPredict4[star1][star2][star3][star4]++;
                // level 5
                if (!starPredict[star1][star2][star3].ContainsKey(star4)) starPredict[star1][star2][star3][star4] = new Dictionary<int, int>();

                // LEVEL 4
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
            Console.WriteLine("- All combinaison 4b : " + countBall_4.Count());
            Console.WriteLine("- All combinaison 3b : " + countBall_3.Count());
            writer.WriteLine("---> Results");
            writer.WriteLine("- All tirages : " + countBall_5_star.Count());
            writer.WriteLine("- All tirages no star : " + countBall_5.Count());
            writer.WriteLine("- All combinaison 4b : " + countBall_4.Count());
            writer.WriteLine("- All combinaison 3b : " + countBall_3.Count());

            // --- Simple stats results
            // best/bad ball numbers
            foreach (int ball in sortedCountEachBalls.Keys)
            {
                if (sortedCountEachBalls[ball] > 80) Console.WriteLine("!! Best ball : " + ball.ToString() + " - nb fois : " + sortedCountEachBalls[ball].ToString());

                if (sortedCountEachBalls[ball] < 60) Console.WriteLine("- Bad ball : " + ball.ToString() + " - nb fois : " + sortedCountEachBalls[ball].ToString());
            }
            // best/bad star numbers
            foreach (int star in sortedCountEachStars.Keys)
            {
                if (sortedCountEachStars[star] > 70) Console.WriteLine("?? Best star : " + star.ToString() + " - nb fois : " + sortedCountEachStars[star].ToString());

                if (sortedCountEachStars[star] < 50) Console.WriteLine("- Bad star : " + star.ToString() + " - nb fois : " + sortedCountEachStars[star].ToString());
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
                if (countBall_4[tirage] > 2) Console.WriteLine("Combinaison 4b : " + tirage + " - nb fois : " + countBall_4[tirage].ToString());
            }
            // best 4 combinaison
            foreach (string tirage in countBall_3.Keys)
            {
                writer.WriteLine(tirage);
                if (countBall_3[tirage] > 2) Console.WriteLine("Combinaison 3b : " + tirage + " - nb fois : " + countBall_3[tirage].ToString());
            }

            // --- Predictor results
            Console.WriteLine("\n---> Predictor best suite");
            // star prediction
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

            // BAD PREDICTION - 1
            // Best ball suite prediction
            Console.WriteLine("\n---> FIRST BAD PREDICTION");
            Console.WriteLine("--> Predictor bad predicted suite");
            List<string> badBallsPredResult = PredictBadBallSuite();
            foreach (string line in badBallsPredResult) { Console.WriteLine(line); }

            // PREDICTION - 1
            // Best star suite prediction
            Console.WriteLine("\n---> FIRST PREDICTION");
            Console.WriteLine("--> Predictor best predicted suite");
            int bestS1 = starPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            List<string> starPredResult = PredictBestStarSuite();
            foreach(string line in starPredResult) { Console.WriteLine(line); }

            // Best ball suite prediction
            Console.WriteLine("\n--> Predictor best predicted suite");
            int bestB1 = ballPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            List<string> ballsPredResult = PredictBestBallSuite();
            foreach (string line in ballsPredResult) { Console.WriteLine(line); }

            // PREDICTION - 2
            starPredict1.Remove(bestS1);
            ballPredict1.Remove(bestB1);
            Console.WriteLine("\n---> SECOND PREDICTION");
            Console.WriteLine("--> Predictor best predicted suite");
            bestS1 = starPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            starPredResult = PredictBestStarSuite();
            foreach (string line in starPredResult) { Console.WriteLine(line); }

            // Best ball suite prediction
            Console.WriteLine("\n--> Predictor best predicted suite");
            bestB1 = ballPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            ballsPredResult = PredictBestBallSuite();
            foreach (string line in ballsPredResult) { Console.WriteLine(line); }

            // PREDICTION - 3
            starPredict1.Remove(bestS1);
            ballPredict1.Remove(bestB1);
            Console.WriteLine("\n---> THIRD PREDICTION");
            Console.WriteLine("--> Predictor best predicted suite");
            bestS1 = starPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            starPredResult = PredictBestStarSuite();
            foreach (string line in starPredResult) { Console.WriteLine(line); }

            // Best ball suite prediction
            Console.WriteLine("\n--> Predictor best predicted suite");
            bestB1 = ballPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            ballsPredResult = PredictBestBallSuite();
            foreach (string line in ballsPredResult) { Console.WriteLine(line); }

            // PREDICTION - 4
            starPredict1.Remove(bestS1);
            ballPredict1.Remove(bestB1);
            Console.WriteLine("\n---> FOURTH PREDICTION");
            Console.WriteLine("--> Predictor best predicted suite");
            bestS1 = starPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            starPredResult = PredictBestStarSuite();
            foreach (string line in starPredResult) { Console.WriteLine(line); }

            // Best ball suite prediction
            Console.WriteLine("\n--> Predictor best predicted suite");
            bestB1 = ballPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            ballsPredResult = PredictBestBallSuite();
            foreach (string line in ballsPredResult) { Console.WriteLine(line); }

            // PREDICTION - 5
            starPredict1.Remove(bestS1);
            ballPredict1.Remove(bestB1);
            Console.WriteLine("\n---> FIVE PREDICTION");
            Console.WriteLine("--> Predictor best predicted suite");
            bestS1 = starPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            starPredResult = PredictBestStarSuite();
            foreach (string line in starPredResult) { Console.WriteLine(line); }

            // Best ball suite prediction
            Console.WriteLine("\n--> Predictor best predicted suite");
            bestB1 = ballPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            ballsPredResult = PredictBestBallSuite();
            foreach (string line in ballsPredResult) { Console.WriteLine(line); }
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

    static private bool Is4CombinaisonFound(List<int> allBallsNumberNoStar, Dictionary<string, int> countBall_4) {
        bool isCombinaisonFound = false;

        List<int> newCombinaison = new List<int>();
        foreach (int ball1 in allBallsNumberNoStar)
        {
            newCombinaison.Add(ball1);

            // Second ball
            foreach (int ball2 in allBallsNumberNoStar)
            {
                if (!newCombinaison.Contains(ball2))
                {
                    newCombinaison.Add(ball2);

                    // Third ball
                    foreach (int ball3 in allBallsNumberNoStar)
                    {
                        if (!newCombinaison.Contains(ball3))
                        {
                            newCombinaison.Add(ball3);

                            // Fourth ball
                            foreach (int ball4 in allBallsNumberNoStar)
                            {
                                if (!newCombinaison.Contains(ball4))
                                {
                                    newCombinaison.Add(ball4);
                                    newCombinaison.Sort();

                                    string combinaison = newCombinaison[0].ToString() + "-" + newCombinaison[1].ToString() + "-" + newCombinaison[2].ToString() + "-" + newCombinaison[3].ToString();

                                    if (countBall_4.ContainsKey(combinaison)) isCombinaisonFound = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        return isCombinaisonFound;
    }

    static private List<string> PredictBestBallSuite() {
        List<string> results = new List<string>();

        Dictionary<int, int> ballPredict1Copy = new Dictionary<int, int>(ballPredict1);
        Dictionary<int, Dictionary<int, int>> ballPredict2Copy = new Dictionary<int, Dictionary<int, int>>(ballPredict2);
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> ballPredict3Copy = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>(ballPredict3);
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> ballPredict4Copy = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>(ballPredict4);
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>> ballPredictCopy = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>>(ballPredict);

        // Best ball suite prediction
        // B1
        List<int> predictedTirage = new List<int>();
        int bestB1 = ballPredict1Copy.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        predictedTirage.Add(bestB1);

        // B2
        ballPredict2Copy[bestB1].Remove(bestB1);
        int bestB2 = ballPredict2Copy[bestB1].Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        predictedTirage.Add(bestB2);

        // B3
        ballPredict3Copy[bestB1][bestB2].Remove(bestB1);
        ballPredict3Copy[bestB1][bestB2].Remove(bestB2);
        int bestB3 = ballPredict3Copy[bestB1][bestB2].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        predictedTirage.Add(bestB3);

        // B4
        ballPredict4Copy[bestB1][bestB2][bestB3].Remove(bestB1);
        ballPredict4Copy[bestB1][bestB2][bestB3].Remove(bestB2);
        ballPredict4Copy[bestB1][bestB2][bestB3].Remove(bestB3);
        int bestB4 = ballPredict4Copy[bestB1][bestB2][bestB3].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        predictedTirage.Add(bestB4);

        // B5
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB1);
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB2);
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB3);
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB4);
        int bestB5 = ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        predictedTirage.Add(bestB5);
        results.Add("/!\\ Best Ball Combinaison prediction : " + bestB1.ToString() + " -> " + bestB2.ToString() + " -> " + bestB3.ToString() + " -> " + bestB4.ToString() + " - " + bestB5.ToString());
        results.Add("/!\\ Respective chances : " + ballPredict1[bestB1].ToString() + " - " + ballPredict2[bestB1][bestB2].ToString() + " - " + ballPredict3[bestB1][bestB2][bestB3].ToString() + " - " + ballPredict4[bestB1][bestB2][bestB3][bestB4].ToString() + " - " + ballPredict[bestB1][bestB2][bestB3][bestB4][bestB5].ToString());
        if (Is4CombinaisonFound(predictedTirage, countBall_4)) results.Add("° Tirage déjà sortit en combinaiso de 4 °");

        return results;
    }

    static private List<string> PredictBadBallSuite()
    {
        List<string> results = new List<string>();

        Dictionary<int, int> ballPredict1Copy = new Dictionary<int, int>(ballPredict1);
        Dictionary<int, Dictionary<int, int>> ballPredict2Copy = new Dictionary<int, Dictionary<int, int>>(ballPredict2);
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> ballPredict3Copy = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>(ballPredict3);
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> ballPredict4Copy = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>(ballPredict4);
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>> ballPredictCopy = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>>(ballPredict);

        // Best ball suite prediction
        // B1
        List<int> predictedTirage = new List<int>();
        int bestB1 = ballPredict1Copy.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
        predictedTirage.Add(bestB1);

        // B2
        ballPredict2Copy[bestB1].Remove(bestB1);
        int bestB2 = ballPredict2Copy[bestB1].Aggregate((a, b) => a.Value < b.Value ? a : b).Key;
        predictedTirage.Add(bestB2);

        // B3
        ballPredict3Copy[bestB1][bestB2].Remove(bestB1);
        ballPredict3Copy[bestB1][bestB2].Remove(bestB2);
        int bestB3 = ballPredict3Copy[bestB1][bestB2].Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
        predictedTirage.Add(bestB3);

        // B4
        ballPredict4Copy[bestB1][bestB2][bestB3].Remove(bestB1);
        ballPredict4Copy[bestB1][bestB2][bestB3].Remove(bestB2);
        ballPredict4Copy[bestB1][bestB2][bestB3].Remove(bestB3);
        int bestB4 = ballPredict4Copy[bestB1][bestB2][bestB3].Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
        predictedTirage.Add(bestB4);

        // B5
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB1);
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB2);
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB3);
        ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Remove(bestB4);
        int bestB5 = ballPredictCopy[bestB1][bestB2][bestB3][bestB4].Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
        predictedTirage.Add(bestB5);
        results.Add("/!\\ Best Ball Combinaison prediction : " + bestB1.ToString() + " -> " + bestB2.ToString() + " -> " + bestB3.ToString() + " -> " + bestB4.ToString() + " - " + bestB5.ToString());
        results.Add("/!\\ Respective chances : " + ballPredict1[bestB1].ToString() + " - " + ballPredict2[bestB1][bestB2].ToString() + " - " + ballPredict3[bestB1][bestB2][bestB3].ToString() + " - " + ballPredict4[bestB1][bestB2][bestB3][bestB4].ToString() + " - " + ballPredict[bestB1][bestB2][bestB3][bestB4][bestB5].ToString());
        if (Is4CombinaisonFound(predictedTirage, countBall_4)) results.Add("° Tirage déjà sortit en combinaiso de 4 °");

        return results;
    }

    static private List<string> PredictBestStarSuite() {
        List<string> results = new List<string>();

        int bestS1 = starPredict1.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        int bestS2 = starPredict2[bestS1].Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        int bestS3 = starPredict3[bestS1][bestS2].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        int bestS4 = starPredict4[bestS1][bestS2][bestS3].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        int bestS5 = starPredict[bestS1][bestS2][bestS3][bestS4].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        results.Add("/!\\ Best Secret Suite prediction : " + bestS1.ToString() + " -> " + bestS2.ToString() + " -> " + bestS3.ToString() + " -> " + bestS4.ToString() + " - " + bestS5.ToString());
        results.Add("/!\\ Respective chances : " + starPredict1[bestS1].ToString() + " - " + starPredict2[bestS1][bestS2].ToString() + " - " + starPredict3[bestS1][bestS2][bestS3].ToString() + " - " + starPredict4[bestS1][bestS2][bestS3][bestS4].ToString() + " - " + starPredict[bestS1][bestS2][bestS3][bestS4][bestS5].ToString());

        return results;
    }
}