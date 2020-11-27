using System;
using System.Collections.Generic;
using System.Threading;
using TowerOfHanoiLibrary;
using static System.Console;

namespace DaliaTowerUI
{
    class Program
    {
        static void Main(string[] args)
        {
            bool continueYN = true;
            TowerUI ui = null;

            do
            {
                Console.Clear();
                ui = new TowerUI();

                int discs = ui.InputNumberOfDiscs();
                if (ui.CreateTowers())
                {
                    SolvingMethod solvingMethod = ui.InputSolvingMethod();

                    if (solvingMethod == SolvingMethod.Manual)
                    {
                        ui.InputMoveAction();
                    }
                    else if (solvingMethod == SolvingMethod.Auto)
                    {
                        ui.AutoSolve();
                    }
                    else if (solvingMethod == SolvingMethod.AutoSolveStepByStep)
                    {
                        ui.AutoSolveStepByStep();
                    }

                    // List Moves
                    ui.ViewMoveRecords();
                }

                // Ask to continue
                Write("\n\nWant to try again (press 'Y' to continue): ");
                string inputStr = ReadKey().KeyChar.ToString().ToUpper();
                continueYN = inputStr == "Y" ? true : false;
            } while (continueYN);

            ui = null;
            WriteLine("\n\nBye. See you again! Press any key to finish up.");
            ReadKey();
            return;
        }
    }

    class TowerUI
    {
        public int NumberOfDiscs { get; set; } 
        private Towers towers = null;
        private Queue<MoveRecord> moveRecords = null;
        private Stack<MoveRecord> undoMoves = null;
        private Stack<MoveRecord> redoMoves = null;

        public TowerUI() { }

        public int InputNumberOfDiscs()
        {
            int discs = 0;
            int defaultDiscs = 3;
            bool valid = false;
            string endMsg = "\nNumber of discs ";
            do
            {
                Write($"Enter Number of discs in your tower (default is {defaultDiscs}, max is 9): ");
                ConsoleKeyInfo consoleKeyInfo = ReadKey();
                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    discs = defaultDiscs;
                    endMsg += $"defaulting to {defaultDiscs}. ";
                    valid = true;
                }
                else
                {
                    string inputStr = consoleKeyInfo.KeyChar.ToString();
                    valid = Int32.TryParse(inputStr, out discs);
                    if (!valid)    // || (valid && (discs < 1 || discs > 9)))  //  Handled cause of InvalidHeightException 
                    {
                        WriteLine("\nInvalid Input: Enter a number between 1-9.");
                        valid = false;
                    }
                    else
                    {
                        endMsg += $"selected is {discs}. ";
                    }
                }

                NumberOfDiscs = discs;

            } while (!valid);

            Write(endMsg + " Press any key to continue.\n");
            ReadKey();

            return discs;
        }

        public SolvingMethod InputSolvingMethod()
        {
            bool valid = false;
            string inputStr;
            SolvingMethod solvingMethod = SolvingMethod.None;

            Console.Clear();
            TowerUtilities.DisplayTowers(GetTowers());

            WriteLine("\n\nOptions: ");
            WriteLine("- M - Solve the puzzle manually");
            WriteLine("- A - Auto-solve");
            WriteLine("- S - Auto-solve Step by Step");

            do
            {
                inputStr = string.Empty;
                Write($"\nChoose an approach: ");
                inputStr = ReadKey().KeyChar.ToString().Trim().ToUpper();
                valid = inputStr == "M" || inputStr == "A" || inputStr == "S" ? true : false;
                if (!valid)    
                {
                    WriteLine("\nInvalid Input: Press \"M\" or \"A\" or \"S\" to choose an approach.");
                    valid = false;
                }
                
            } while (!valid);

            switch (inputStr)
            {
                case "M":
                    solvingMethod = SolvingMethod.Manual;
                    break;
                case "A":
                    solvingMethod = SolvingMethod.Auto;
                    break;
                case "S":
                    solvingMethod = SolvingMethod.AutoSolveStepByStep;
                    break;
            }

            return solvingMethod;
        }

        public bool CreateTowers()
        {
            /** Handled cause of InvalidHeightException
            if (NumberOfDiscs <= 0)
            {
                InputNumberOfDiscs();
            }
            */
            try
            {
                towers = new Towers(NumberOfDiscs);
                moveRecords = new Queue<MoveRecord>();
                undoMoves = new Stack<MoveRecord>();
                redoMoves = new Stack<MoveRecord>();
            }
            catch (InvalidHeightException he)
            {
                WriteLine($"\n InvalidHeightException: {he.Message}");
                return false;
            }
            return true;
        }

        public Towers GetTowers() => towers;

        public void ViewMoveRecords()
        {
            string viewYN;
            do
            {
                Write("\n\nWould you like to see a list of the moves you made? ('Y' or 'N'): ");
                viewYN = ReadKey().KeyChar.ToString().ToUpper();
            } while (viewYN != "Y" && viewYN != "N");

            if (viewYN == "N")
                return;

            WriteLine("\n");
            if (moveRecords.Count == 0)
            {
                WriteLine("No moves to display.");
            }
            else
            {
                int i = 1;
                foreach (MoveRecord record in moveRecords)
                {
                    WriteLine($"   {i++}. Disc {record.DiscNumber} moved from tower {record.FromPole} to tower {record.ToPole}");
                }
            }

            return;
        }

        #region "Manual Solve"

        // Manual Play
        public int InputMoveAction()
        {
            string inputStr;
            int fromPole = 0, toPole = 0, moveNum;
            bool valid = false;
            bool moveCancelled = false;
            bool undoOper = false, redoOper = false;

            Console.Clear();
            TowerUtilities.DisplayTowers(GetTowers());

            do
            {
                fromPole = 0; toPole = 0;
                moveCancelled = false;

                moveNum = towers.NumberOfMoves +1;
                WriteLine($"\nMove {moveNum}:");

                // From tower Input
                do
                {
                    
                    Write($"\nEnter 'from' tower number (1 – 3), {GetUndoRedoMessagePart()} or ‘x’ to quit : ");
                    inputStr = ReadKey().KeyChar.ToString().Trim().ToUpper();
                    undoOper = inputStr == "\u001a" ? true : false;     // Undo operation
                    redoOper = inputStr == "\u0019" ? true : false;     // Redo operation
                    if (inputStr == "X")
                    {
                        WriteLine($"\nWell, you hand in there fo {towers.NumberOfMoves} moves. Nice try.");
                        return -1;
                    }

                    if (undoOper || redoOper)
                    {
                        valid = true;
                    }
                    else
                    {
                        // From Pole
                        valid = IsPoleInputValid(inputStr, out fromPole);
                        if (!valid)
                        {
                            WriteLine("\nInvalid value. Tower value should be (1 – 3) or press 'Ctrl+z' or 'Ctrl+y' or 'x'");
                        }
                    }
                } while (!valid);

                // To tower Input
                if (!undoOper && !redoOper)
                {
                    do
                    {
                        Write("\nEnter 'to' tower number (1 – 3), or enter to cancel : ");
                        ConsoleKeyInfo consoleKeyInfo = ReadKey();
                        if (consoleKeyInfo.Key == ConsoleKey.Enter)
                        {
                            moveCancelled = true;
                            WriteLine("\nMove cancelled.");
                            break;
                        }
                        else
                        {
                            inputStr = consoleKeyInfo.KeyChar.ToString().Trim().ToUpper();
                            valid = IsPoleInputValid(inputStr, out toPole);
                            if (!valid)
                            {
                                WriteLine("\nInvalid value. Tower value should be (1 – 3) or press 'enter'");
                            }
                        }
                    } while (!valid);
                }

                if(moveCancelled)
                {
                    continue;
                }

                // Move operation - Undo
                if (undoOper)
                {
                    UndoMove();
                }
                // Move operation - ReDo
                else if (redoOper)  
                {
                    RedoMove();
                }
                // Move operation - normally
                else
                {
                    NormalMove(fromPole, toPole);
                }

            } while (! towers.IsComplete);

            return 0;
        }

        private bool IsPoleInputValid(string poleStr, out int poleNum)
        {
            bool valid = Int32.TryParse(poleStr, out poleNum);
            if (valid && (poleNum < 1 || poleNum > 3))
            {
                valid = false;
            }

            return valid;
        }

        private string GetUndoRedoMessagePart()
        {
            string msg = "";
            if (undoMoves.Count > 0 && undoMoves.Peek() != null)
            {
                msg += "\"Ctrl + z\" to undo, ";
            }
            if (redoMoves.Count > 0 && redoMoves.Peek() != null)
            {
                msg += "\"Ctrl + y\" to redo, ";
            }

            return msg;
        }

        private void UndoMove()
        {
            // Move operation - Undo
            try
            {
                if (undoMoves.Count == 0 || undoMoves.Peek() == null)
                {
                    throw new InvalidOperationException("No moves to undo.");
                }

                // Get record from undo stack
                MoveRecord recordToUndo = undoMoves.Pop();
                // Make the move
                MoveRecord record = towers.Move(recordToUndo.ToPole, recordToUndo.FromPole);
                // Add the move to queue
                moveRecords.Enqueue(record);
                // Push it to redo stack
                redoMoves.Push(recordToUndo);

                Console.Clear();
                TowerUtilities.DisplayTowers(GetTowers());
                WriteLine($"\nMove {towers.NumberOfMoves} complete by undo of move {recordToUndo.MoveNumber}. Disc {recordToUndo.DiscNumber} restored to tower {recordToUndo.FromPole} from tower {recordToUndo.ToPole}");
            }
            catch (Exception ex)
            {
                WriteLine($"\n{ex.GetType().Name}: {ex.Message}");
            }

            return;
        }

        private void RedoMove()
        {
            try
            {
                if (redoMoves.Count == 0 || redoMoves.Peek() == null)
                {
                    throw new InvalidOperationException("No moves to Redo.");
                }

                // Get record from redo stack
                MoveRecord recordToUndo = redoMoves.Pop();
                // Make the move
                MoveRecord record = towers.Move(recordToUndo.FromPole, recordToUndo.ToPole);
                // Add the move to queue
                moveRecords.Enqueue(record);
                // Push it to undo stack
                undoMoves.Push(recordToUndo);

                Console.Clear();
                TowerUtilities.DisplayTowers(GetTowers());
                WriteLine($"\nMove {towers.NumberOfMoves} complete by redo of move {recordToUndo.MoveNumber}. Disc {recordToUndo.DiscNumber} returned to tower {recordToUndo.ToPole} from tower {recordToUndo.FromPole}");
            }
            catch (Exception ex)
            {
                WriteLine($"\n{ex.GetType().Name}: {ex.Message}");
            }

            return;
        }

        private void NormalMove(int fromPole, int toPole)
        {
            try
            {
                MoveRecord record = towers.Move(fromPole, toPole);
                moveRecords.Enqueue(record);
                undoMoves.Push(record);
                redoMoves.Clear();

                Console.Clear();
                TowerUtilities.DisplayTowers(GetTowers());
                WriteLine($"\nMove {towers.NumberOfMoves} complete. Successfully moved disc from tower {fromPole} to tower {toPole}");

                // Game is complete
                if (towers.IsComplete)
                {
                    string msg = towers.NumberOfMoves == towers.MinimumPossibleMoves ?
                        $"It took you {towers.NumberOfMoves} moves. Congrats! That's the minimum!" :
                        $"It took you {towers.NumberOfMoves} moves. Not bad, but it can be done in {towers.MinimumPossibleMoves} moves. Try again.";

                    WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                WriteLine($"\n{ex.GetType().Name}: {ex.Message}");
            }

            return;
        }

        #endregion

        #region "Auto Solve Recursively"
        public void AutoSolve()
        {
            Console.Clear();
            TowerUtilities.DisplayTowers(GetTowers());

            Write("\nPress a key and watch closely!\n");
            ReadKey();

            // Start
            MoveAuto(NumberOfDiscs, 1, 2, 3);

            return;
        }

        private void MoveAuto(int discNumber, int fromPole, int auxPole, int toPole)
        {
            if (discNumber == 1)
            {
                Console.Clear();
                MoveRecord record = towers.Move(fromPole, toPole);
                moveRecords.Enqueue(record);
                TowerUtilities.DisplayTowers(GetTowers());
                WriteLine($"\nMove {towers.NumberOfMoves} complete. Successfully moved disc from tower {fromPole} to tower {toPole}");
                if (towers.IsComplete)
                {
                    WriteLine($"Number of Moves: {towers.NumberOfMoves}");
                }
                Thread.Sleep(500);

                return;
            }
            else
            {
                MoveAuto(discNumber - 1, fromPole, toPole, auxPole);
                
                Console.Clear();
                MoveRecord record = towers.Move(fromPole, toPole);
                moveRecords.Enqueue(record);
                TowerUtilities.DisplayTowers(GetTowers());
                WriteLine($"\nMove {towers.NumberOfMoves} complete. Successfully moved disc from tower {fromPole} to tower {toPole}");
                Thread.Sleep(500);

                MoveAuto(discNumber - 1, auxPole, fromPole, toPole);
            }
        }

        #endregion

       #region "AutoSolveStepByStep"

        public void AutoSolveStepByStep()
        {
            Console.Clear();
            TowerUtilities.DisplayTowers(GetTowers());

            Write("\nPress a key to see the first move.\n");
            ReadKey();

            MoveStepByStep();

            return;
        }

        private void MoveStepByStep()
        {
            int denominator = 0;
            int i = 0;
            int fromPole = 1;
            int auxPole = 2;
            int toPole = 3;

            // For even # of disks, interchange the auxPole & toPole
            if (towers.NumberOfDiscs % 2 == 0)
            {
                int temp = auxPole;
                auxPole = toPole;
                toPole = temp;
            }

            for (i = 1; i <= towers.MinimumPossibleMoves; i++)
            {
                denominator = i % 3;
                switch (denominator)
                {
                    case 0:
                        MoveStep(auxPole, toPole);
                        break;
                    case 1:
                        MoveStep(fromPole, toPole);
                        break;
                    case 2:
                        MoveStep(fromPole, auxPole);
                        break;
                }

                if (i < towers.MinimumPossibleMoves) { 
                    Write("\nPress any key to move to the next step or \"X\" to exit");
                    if (ReadKey().KeyChar.ToString().Trim().ToUpper() == "X")
                    {
                        WriteLine($"\n\nStep-through aborted. Number of Moves: {towers.NumberOfMoves}");
                        break;
                    }
                }
            }

            if (i >= towers.MinimumPossibleMoves)
            {
                if (towers.IsComplete)
                {
                    WriteLine($"Step-through completed. Number of Moves: {towers.NumberOfMoves}");
                }
                else
                {
                    WriteLine($"OOPS! Something went wrong...Game should be complete by {towers.NumberOfMoves} moves");
                }
            }

            return;
        }

        private void MoveStep(int fromPole, int toPole)
        {
            MoveRecord record = null;
            int source = fromPole, dest = toPole;
            int[][] towersArr = towers.ToArray();

            // Interchange fromPole & toPole, if required
            // If frompole is empty
            if (towersArr[fromPole-1].Length == 0)
            {
                source = toPole;
                dest = fromPole;
            }
            // When top disk of fromPole > top disk of toPole
            else if (towersArr[toPole - 1].Length > 0 && (towersArr[fromPole - 1][0] > towersArr[toPole - 1][0]) )
            {
                source = toPole;
                dest = fromPole;
            }

            record = towers.Move(source, dest);
            if (record != null)
            {
                moveRecords.Enqueue(record);
                TowerUtilities.DisplayTowers(GetTowers());
                WriteLine($"\nMove {towers.NumberOfMoves} complete. Successfully moved disc from tower {source} to tower {dest}");
            } 
            else
            {
                WriteLine("ERROR: Fail to move - record is null");
            }
        }

        #endregion
    }

    enum SolvingMethod
    {
        None,
        Manual, 
        Auto, 
        AutoSolveStepByStep
    };
}