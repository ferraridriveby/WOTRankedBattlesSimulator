using System;

namespace WGRankedSim
{
    class Program
    {
        static Random r = new Random();

        //WG's bracket league system
        static int[] brackets = new int[] { 300, 400, 500, 600, 700, 800, 900, 1000 };


        static void Main(string[] args)
        {
            //number of trials to run
            int games = 1000000;

            //average player

            
            int winrate = 50;

            double[] probability = {
                100.0 / 7, //1st
                100.0 / 7,
                100.0 / 7,
                100.0 / 7,
                100.0 / 7,
                100.0 / 7,
                100.0 / 7
            };
            

            /*//uni player

            int winrate = 55;

            double[] probability = {
                20, //1st
                20,
                20,
                10,
                10,
                10,
                10
            };
            

            int winrate = 65;

            double[] probability = {
                5, //1st
                20,
                30,
                20,
                10,
                10,
                5
            };
            */

            //run simulations

            double avgOldGames = runOldSimulation(games, winrate, probability[0]);
            Console.WriteLine("Average Old Games {0}", avgOldGames);

            double avgNewGames = runNewSimulation(100000, winrate, probability);
            Console.WriteLine("Average New Games {0}", avgNewGames);
        }

        static double runOldSimulation(int trials, double avgWinRate, double avgTopRate)
        {
            //collate results
            int[] results = new int[trials];

            for(int i = 0; i < trials; i++)
            {
                int totalPoints = 0; //sanity check -- adding to 5200 is correct. <--- this passed.

                //default player configuration
                int curPoints = 0;
                int curBracket = 0;

                int totalGames = 0;

                //run simulation until final league is reached
                do
                {
                    //win = under probability distri of rate
                    bool win = r.Next(1, 101) <= avgWinRate;

                    if (win)
                    {
                        curPoints += 100;
                        totalPoints += 100;
                    }
                    else
                    {
                        //top roll is separate from win roll (for obvious reasons)
                        bool top = r.Next(1, 101) <= avgTopRate;
                        if (curPoints > 0 && !top)
                        {
                            curPoints -= 100;
                            totalPoints -= 100;
                        }
                    }

                    totalGames++;

                    if (curPoints == brackets[curBracket])
                    {
                        curPoints = 100;
                        curBracket++;
                    }
                }
                while (curBracket != brackets.Length);

                results[i] = totalGames; 
                
            }

            //compute average number of games

            double avgGames = 0;

            for(int i = 0; i < trials; i++)
            {
                avgGames += results[i];
            }

            return avgGames / trials;

        }

        //WG's new system
        //static int[] winPoints = new int[] { 100, 85, 70, 55, 40, 25, 10 };
        //static int[] lossPoints = new int[] { 0, -15, -30, -45, -60, -75, -90 };

        static int[] winPoints = new int[] { 100, 100, 100, 100, 50, 50, 50 };
        static int[] lossPoints = new int[] { 0, -50, -50, -50, -100, -100, -100 };

        static double runNewSimulation(int trials, double avgWinRate, double[] probSet)
        {
            int[] results = new int[trials];

            for (int i = 0; i < trials; i++)
            {
                int curPoints = 0;
                int curBracket = 0;

                int totalGames = 0;

                do
                {
                    bool win = r.Next(1, 101) <= avgWinRate;
                    double placevalue = r.Next(1, 101);

                    double sumprob = 0;

                    int spot = 0;

                    for(int j = 0; j < 7; j++)
                    {
                        sumprob += probSet[j];
                        if (sumprob > placevalue)
                        {
                            spot = j;
                            break;
                        }
                    }

                    if (win)
                    {
                        curPoints += winPoints[spot];
                    }
                    else
                    {
                        //only deduct points if it brings us to zero
                        if(curPoints + lossPoints[spot] >= 0)
                            curPoints += lossPoints[spot];
                    }

                    totalGames++;

                    //need to check greater condition here since it's no longer a clean divisor of 100
                    if (curPoints >= brackets[curBracket])
                    {
                        curPoints = 100;
                        curBracket++;
                    }
                }
                while (curBracket != brackets.Length);

                results[i] = totalGames;

            }

            double avgGames = 0;

            for (int i = 0; i < trials; i++)
            {
                avgGames += results[i];
            }

            return avgGames / trials;

        }



    }
}
