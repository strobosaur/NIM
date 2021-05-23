using System;
using System.Linq;

namespace GLP_HT20_P2_Nim
{
    class Program
    {
        static int maxPlayers = 2;
        static int pileSize = 5;
        static int pileCount = 3;

        static int[] gameBoard = new int[pileCount];
        static int[] scoreBoard = new int[maxPlayers];
        static string[] playerList = new string[maxPlayers];
        static bool[] playerIsAI = new bool[maxPlayers];
        static ConsoleColor[] nameColorArr = new ConsoleColor[] {ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Cyan};

        static int playerTurn;
        static int numberOfGames;
        static int numberOfTurns;

        static ConsoleColor backgroundColor = ConsoleColor.DarkMagenta;

        static Random rng = new Random();

        static void Main(string[] args)
        {
            // UPDATE CONSOLE BACKGROUND COLOR
            Console.BackgroundColor = backgroundColor;
            Console.Clear();

            // DISPLAY MENU & RUN GAME
            do
            {
                Meny();
                NewGame();

                // ASK FOR NEW GAME
                Console.Clear();
                Console.Write("\n\n\n\tNytt spel? (j/n): ");

            } while (Console.ReadLine() != "n");

            // BE POLITE
            Console.Clear();
            Console.WriteLine("\n\n\n\t# GAME OVER #\n\tTack för att du spelade Nim!\n\n\n\n\n");
        }

        /// <summary>
        /// Hälsar spelaren välkommen och förklarar spelets regler
        /// Spelaren får välja om denne är redo att möta Nim's utmaningar,
        /// samt om spelaren vill skräddarsy spelets regler med CustomGame()
        /// sedan drar det igång
        /// </summary>
        static void Meny()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Clear();

            Console.WriteLine("\n\tVälkommen till Nim!");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("\n\tSPELREGLER: Spelplanen består av tre högar med 5 stickor i varje hög. " +
                            "\n\tVarje spelare får vid sin tur välja en hög och valfritt antal stickor " +
                            "\n\tsom ska plockas från den valda högen. Tillslut kommer bara en hög " +
                            "\n\tfinnas kvar. Den spelare som tar slut på stickorna i den sista högen " +
                            "\n\tvinner spelet!");
            Console.ReadLine();
            Console.Clear();

            bool inputOk = false;
            do
            {
                Console.Write("\n\tÄr du redo att spela? (j/n): ");
                string villSpela = Console.ReadLine();
                Console.WriteLine();

                if (villSpela == "n")
                {
                    Environment.Exit(0);

                }
                else if (villSpela == "j")
                {
                    Console.WriteLine("\tYay! Då kör vi!");
                    Console.ReadLine();
                    inputOk = true;
                }
                else
                {
                    Console.WriteLine("\tFör taggad för att skriva ja? Försök igen!");
                    inputOk = false;
                }
            } while (!inputOk);

            // ASK IF PLAYER WANTS TO PLAY A CUSTOM GAME
            Console.Clear();
            Console.Write("\n\tVill du skräddarsy spelets regler (custom game)? (j/n): ");
            if (Console.ReadLine() == "j")
                CustomGame();
        }

        /// <summary>
        /// Loopar igenom max antal spelare,
        /// läser in namn från spelaren 1-15 tecken,
        /// frågar om denna spelare är AI,
        /// och om ja väljs, sätter spelarens index
        /// i bool arrayen playerIsAI till true
        /// </summary>
        static void RegisterPlayers()
        {
            Console.Clear();
            Console.WriteLine();
            string inputChoice = "";

            // LOOP UNTIL ALL PLAYERS ADDED
            for(int i = 0; i < maxPlayers; i++)
            {
                // INPUT PLAYER NAME, 1 - 15 CHARS
                do
                {
                    Console.Write($"\tSkriv in namn på spelare {i + 1}: ");
                    playerList[i] = Console.ReadLine();
                    Console.WriteLine();

                } while ((playerList[i].Length < 1) || (playerList[i].Length > 15));

                // IS THIS PLAYER RUN BY THE AI?
                do
                {
                    Console.Write("\tÄr den här spelaren en AI? (j/n): ");
                    inputChoice = Console.ReadLine();
                    if (inputChoice == "j")
                        playerIsAI[i] = true;                    
                    else if (inputChoice == "n")
                        playerIsAI[i] = false;

                    Console.WriteLine();

                } while ((inputChoice != "n") && (inputChoice != "j"));

                Console.WriteLine("\t================================\n");
            }
        }

        /// <summary>
        /// Uppdaterar samtliga viktiga arrays för spelet
        /// med värden som kan ha uppdaterats i metoden CustomGame()
        /// Anropar RegisterPlayers()
        /// Kör en loop där spelaren som får första tur slumpas,
        /// antalet drag nollställs, fyller gameBoard[] med korrekt antal
        /// pinnar, och kör NewGame(). Efter matchen får spelaren
        /// frågan om denne vill spela en ny match med samma spelare
        /// och regler.
        /// </summary>
        static void NewGame()
        {
            // CREATE CORE GAME ARRAYS
            gameBoard = new int[pileCount];
            scoreBoard = new int[maxPlayers];
            playerList = new string[maxPlayers];
            playerIsAI = new bool[maxPlayers];

            // PLAYER REGISTRATION
            RegisterPlayers();

            // RUN MATCHES UNTIL PLAYER PUKES
            do
            {
                // RESET TURN COUNTER
                numberOfTurns = 0;

                // RANDOMIZE STARTING PLAYER
                playerTurn = rng.Next(0, maxPlayers);

                // FILL GAME BOARD WITH STICKS
                for (int i = 0; i < gameBoard.Length; i++)
                {
                    gameBoard[i] = pileSize;
                }

                // START THE GAME
                NewTurn();

                // ASK FOR NEW ROUND
                Console.Write("\tNy omgång? (j/n): ");
            } while (Console.ReadLine() != "n");
        }

        /// <summary>
        /// Skriver ut totalt antal drag,
        /// skriver sedan ut spelaren som nu har sitt drag
        /// i färgen på spelarens indexplats i färgarrayen.
        /// Ritar sedan upp spelplanen med DrawBoard()
        /// Anropar sedan NewMove() för att göra ett drag.
        /// Kolla till sist vinstvillkoret med CheckWin(),
        /// och om retur är false körs NewTurn() igen.
        /// </summary>
        static void NewTurn()
        {

            // PRINT NR. OF TURNS + CURRENT PLAYER NAME
            Console.Clear();
            Console.WriteLine($"\n\tTotalt antal drag: {numberOfTurns}");

            Console.ForegroundColor = nameColorArr[playerTurn];
            Console.WriteLine($"\t{((playerIsAI[playerTurn]) ? "(AI) " : "")}{playerList[playerTurn]}'s tur\n");
            Console.ResetColor();
            Console.BackgroundColor = backgroundColor;

            // UPPDATE GAME BOARD
            DrawBoard();

            // CURRENT PLAYER MAKES HIS MOVE
            MakeMove(playerTurn);

            // CHECK FOR WIN CONDITION / PLAY NEXT TURN
            if (!CheckWin(playerTurn))
                NewTurn();
        }

        /// <summary>
        /// Loopar igenom gameBoard[] och ritar upp antalet
        /// pinnar i varje hög i färg turkos
        /// </summary>
        static void DrawBoard()
        {
            Console.WriteLine("\t================================\n");

            // LOOP THROUGH PILE ARRAY
            for (int i = 0; i < gameBoard.Length; i++)
            {
                Console.Write($"\tHög {i+1}:    ");

                // OUTPUT NUMBER OF STICKS FROM PILE ARRAY
                for(int j = 0; j < gameBoard[i]; j++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("| ");
                    Console.ResetColor();

                    Console.BackgroundColor = backgroundColor;
                }

                Console.WriteLine("\n");
            }

            Console.WriteLine("\t================================\n");
        }

        /// <summary>
        /// Redovisar matchstatistiken hittills,
        /// varje spelarnamn skrivs ut samt antalet
        /// matcher motsvarande spelare har vunnit
        /// den här omgången.
        /// </summary>
        static void DrawScoreBoard()
        {
            Console.WriteLine("\tMatchstatistik:");
            for (int i = 0; i < scoreBoard.Length; i++)
            {
                Console.WriteLine($"\t{scoreBoard[i]} poäng: {playerList[i]}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Kollar om summan av array gameBoard[] mindre än 1,
        /// (mindre än en pinne kvar på planen)
        /// är den det så är det game over,
        /// antalet spelade spel uppdateras,
        /// och en poäng läggs till i scoreBoard[] på nuvarande
        /// spelares indexposition. Skriver ut poäng
        /// med DrawScoreBoard().
        /// 
        /// Om det finns pinnar kvar på planen så uppdateras
        /// variabeln som håller vilken spelare som har nästa tur.
        /// </summary>
        /// <param name="player"> int värdet för spelaren vars tur det är </param>
        /// <returns> Returnerar true om game over, false om spelet fortsätter </returns>
        static bool CheckWin(int player)
        {
            // IF NO STICKS LEFT = GAME OVER
            if (gameBoard.Sum() < 1)
            {
                // UPDATE NUMBER OF GAMES
                numberOfGames++;

                // PRINT GAME & WINNER INFO
                Console.Clear();
                Console.WriteLine("\n\t# GAME OVER #\n");
                Console.WriteLine($"\t{playerList[player]} vann!");
                Console.WriteLine($"\tTotalt antal drag: {numberOfTurns}\n");
                Console.WriteLine($"\tTotalt antal matcher: {numberOfGames}\n");

                // UPDATE SCORE BOARD ARRAY
                scoreBoard[player]++;

                // DRAW SCORE BOARD
                DrawScoreBoard();

                // RETURN
                return true;
            }
            else
            {
                // IF NOT GAME OVER, NEXT PLAYERS TURN
                playerTurn = (playerTurn + 1) % maxPlayers;

                // RETURN
                return false;
            }
        }

        /// <summary>
        /// Kollar i playerIsAI[] om nuvarande spelare är AI,
        /// gör sedan valet mellan MakeMoveAI()
        /// och MakeMovePlayer()
        /// 
        /// uppdaterar till sist totala antalet drag den här matchen.
        /// </summary>
        /// <param name="player"> int värdet för nuvarande spelare </param>
        static void MakeMove(int player)
        {
            // IF PLAYER IS AI, MAKE AI MOVE
            if (playerIsAI[player])
                MakeMoveAI();

            // IF PLAYER IS HUMAN, MAKE HUMAN MOVE
            else
                MakeMovePlayer();

            // UPDATE NUMBER OF MOVES TOTAL
            numberOfTurns++;
        }

        /// <summary>
        /// Tar input från användaren i formatet 'x x'
        /// Input felkontrolleras så att högar, pinnar och
        /// formattering stämmer.
        /// 
        /// Om formattering stämmer så minskas gameBoard[] med valt antal
        /// pinnar, på vald indexposition i arrayen;
        /// </summary>
        static void MakeMovePlayer()
        {
            bool moveOK = true;
            int pile = 0;
            int sticks = 0;

            //INPUT MOVE LOOP UNTIL INPUT = GOOD
            do
            {
                string moveString;

                Console.Write("\tGör ditt drag: '<hög> <pinnar>' : ");
                moveString = Console.ReadLine();
                Console.WriteLine("\n");

                // CHECK FOR CORRECT INPUT FORMAT
                try
                {
                    pile = 0;
                    sticks = 0;
                    moveOK = true;

                    // SPLIT INPUT STRING
                    string[] moveArr = moveString.Split(' ');

                    // IF INPUT TOO LONG, INPUT = FAIL
                    if (moveArr.Length > 2)
                        moveOK = false;
                    else
                    {
                        // CONVERT INPUT TO INT
                        pile = (Convert.ToInt32(moveArr[0]) - 1);
                        sticks = Convert.ToInt32(moveArr[1]);
                    }

                    // CHECK FOR PILE INPUT INVALID NUMBER
                    if ((pile < 0) || (pile >= gameBoard.Length))
                    {
                        moveOK = false;
                        Console.WriteLine("\tHögen finns inte, försök igen...\n");
                    }
                    // CHECK FOR STICK INPUT INVALID NUMBER
                    else if ((sticks < 1) || (sticks > gameBoard[pile]))
                    {
                        moveOK = false;
                        Console.WriteLine("\tOgiltigt antal pinnar, försök igen...\n");
                    }
                }
                // CHECK FOR UNKNOWN EXCEPTION
                catch (Exception e)
                {
                    Console.WriteLine("\tFelaktigt formaterad input. Glömde du mellanslag mellan siffrorna?\n");
                    moveOK = false;
                }

            } while (!moveOK);

            // MAKE MOVE
            gameBoard[pile] -= sticks;
        }

        /// <summary>
        /// AI:n kollar igenom gameBoard[] efter högar med
        /// fler än noll pinnar och sparar dess indexpositioner i en ny array;
        /// int state uppdateras också beroende på hur många tomma högar som finns.
        /// 
        /// Ett slumpvärde genereras för att slumpa hur agressiv AI:n är.
        /// 
        /// Beroende på hur många högar som finns kvar (int state) gör AI:n olika val.
        /// Antingen tar den alla pinnar i sista högen och vinner,
        /// eller så försöker den lura spelaren till en position där spelaren
        /// måste dra den näst sista stickan = AIn vinner nästa drag.
        /// Om ingen hög är tom väljer AIn en hög på random och ett random
        /// antal pinnar att plocka.
        /// </summary>
        static void MakeMoveAI()
        {
            int state = 0;
            int pile = 0;
            int sticks = 0;
            int[] pilesWithSticks = new int[0];

            // HUMANIZE AI & MAKE THINGS FUN
            string[] AIwaiting = new string[10];
            AIwaiting[0] = $"{playerList[playerTurn]} funderar över sitt drag...";
            AIwaiting[1] = $"{playerList[playerTurn]} kliar sig oroligt i pollisongerna...";
            AIwaiting[2] = $"{playerList[playerTurn]} inspekterar självsäkert spelplanen...";
            AIwaiting[3] = $"{playerList[playerTurn]} verkar djupt förjunken i tankar...";
            AIwaiting[4] = $"{playerList[playerTurn]} plirar frustrerat på dig...";
            AIwaiting[5] = $"{playerList[playerTurn]} knorrar och rynkar pannan...";
            AIwaiting[6] = $"{playerList[playerTurn]} blundar och koncentrerar sig...";
            AIwaiting[7] = $"{playerList[playerTurn]} tar ett djupt andetag...";
            AIwaiting[8] = $"{playerList[playerTurn]} suckar och ber en bön...";
            AIwaiting[9] = $"{playerList[playerTurn]} har en glimt av segervittring...";

            Console.WriteLine($"\t{AIwaiting[rng.Next(0,AIwaiting.Length)]}\n");
            Console.ReadKey();

            // CHECK STICK COUNT IN PILES
            for (int i = 0; i < gameBoard.Length; i++)
            {
                if (gameBoard[i] > 0)
                {
                    // SAVE INDICES FOR NON-EMPTY PILES IN NEW ARRAY
                    pilesWithSticks = ResizeArray(pilesWithSticks, pilesWithSticks.Length + 1);
                    pilesWithSticks[pilesWithSticks.Length - 1] = i;

                    // UPDATE AI STATE
                    if (state < 3)
                        state++;
                }
            }

            // RANDOMIZE AI AGRESSIVENESS
            int rand1 = ((int)((double)rng.Next(0, 3) / 2.0)) + ((int)((double)rng.Next(0, 4) / 3.0));

            // MAKE MOVE
            switch (state)
            {
                // 1 PILE WITH STICKS LEFT
                case 1:
                    // EMPTY LAST PILE & TAKE THE WIN
                    pile = pilesWithSticks[0];
                    sticks = gameBoard[pile];

                    // BREAK
                    break;

                // 2 PILES WITH STICKS LEFT
                case 2:

                    // CHECK FOR PILES WITH SINGLE STICK AND TRY BEING SNEAKY
                    if (gameBoard[pilesWithSticks[0]] == 1)
                    {
                        pile = pilesWithSticks[1];
                        sticks = Math.Max(1, (gameBoard[pile] - 1));
                    }
                    else if (gameBoard[pilesWithSticks[1]] == 1)
                    {
                        pile = pilesWithSticks[0];
                        sticks = Math.Max(1, (gameBoard[pile] - 1));
                    }
                    else
                    {
                        pile = pilesWithSticks[rng.Next(0, pilesWithSticks.Length)];
                        sticks = rng.Next(1, Math.Max(2, (gameBoard[pile] - 2) + rand1) );
                    }

                    // BREAK
                    break;

                // 3 OR MORE PILES WITH STICKS LEFT
                case 3:
                    // PICK PILE & STICKS RANDOMLY
                    pile = pilesWithSticks[rng.Next(0, pilesWithSticks.Length)];
                    sticks = rng.Next(1, Math.Max(2, (gameBoard[pile] - 1) + rand1));

                    // BREAK
                    break;
            }

            // MAKE MOVE
            Console.WriteLine($"\t{playerList[playerTurn]} gör draget: '{pile+1} {sticks}'");
            Console.ReadKey();

            // UPDATE GAME BOARD
            gameBoard[pile] -= sticks;
        }

        /// <summary>
        /// Spelaren får möjlighet att välja hur många spelare,
        /// hur många högar,
        /// och hur många pinnar i varje hög.
        /// </summary>
        static void CustomGame()
        {
            // INPUT NUMBER OF PLAYERS
            do
            {
                Console.Clear();
                Console.WriteLine($"\n\t Spelare: {maxPlayers}\t\tHögar: {pileCount}\t\tPinnar: {pileSize}\n");
                Console.Write("\tSkriv in antal spelare (2-4): ");
                int.TryParse(Console.ReadLine(), out maxPlayers);
                Console.WriteLine();

            } while ((maxPlayers < 2) || (maxPlayers > 4));

            // INPUT NUMBER OF PILES
            do
            {
                Console.Clear();
                Console.WriteLine($"\n\t Spelare: {maxPlayers}\t\tHögar:{pileCount}\t\tPinnar:{pileSize}\n");
                Console.Write("\tSkriv in antal högar (3-9): ");
                int.TryParse(Console.ReadLine(), out pileCount);
                Console.WriteLine();

            } while ((pileCount < 3) || (pileCount > 9));

            // INPUT NUMBER OF PILES
            do
            {
                Console.Clear();
                Console.WriteLine($"\n\tSpelare: {maxPlayers}\t\tHögar:{pileCount}\t\tPinnar:{pileSize}\n");
                Console.Write("\tSkriv in antal pinnar per hög (3-10): ");
                int.TryParse(Console.ReadLine(), out pileSize);
                Console.WriteLine();

            } while ((pileSize < 3) || (pileSize > 10));
        }

        /// <summary>
        /// Standard resize array funktion.
        /// Kan både förstora och förminska en array.
        /// Kopierar värdena upp till lägsta arraylängden till ny array,
        /// och skickar sedan ut den nya arrayen.
        /// </summary>
        /// <param name="arr"> arrayen som skall förstoras/förminskas </param>
        /// <param name="size"> den nya storleken på arrayen </param>
        /// <returns> den nya arrayen med nya storleken </returns>
        static int[] ResizeArray(int[] arr, int size)
        {
            int[] newArr = new int[size];
            int iterations = Math.Min(size, arr.Length);

            for (int i = 0; i < iterations; i++)
            {
                newArr[i] = arr[i];
            }

            return newArr;
        }
    }
}
