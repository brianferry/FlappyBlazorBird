using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// THIS CODE IS "DIRECT TRANSLATION" FROM PYTHON PYGAME TO C# BLAZOR. REFACTOR PENDING

namespace FlappyBlazorBird.Client.Data
{
    public class Universe: Printable
    {
        public Universe():base()
        {
            (upperPipes, lowerPipes) = GetNewPipes();
            StartedAt = DateTime.Now.ToString();
            MainLoop();
        }

        public int CurrentFps = 0;
        public string StartedAt;
        public long TotalSessions = 0;
        public int MaxScore = 0;
        public async void MainLoop()
        {
            while (true)
            {
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Stop();                
                stopWatch.Start();
                this.Recalcula();
                this.OnTic();                
                var d = this.FPS_DELAY - stopWatch.Elapsed.Milliseconds;
                if (d<=1) d = 1;
                await Task.Delay(d);                
                CurrentFps = Convert.ToInt32(1000.0 / stopWatch.Elapsed.Milliseconds);
            }        
        }
        public int loopIter = 0;
        public int basex = 0;
        public int baseShift => this.GetBaseWidth - this.GetBackgroundWidth;

        private void Recalcula()
        {
            foreach(var player in Players)
            {
                player.Tic();
            }

            // playerIndex basex change

            loopIter = (loopIter + 1) % 30;
            basex = -((-basex + 100) % baseShift);

            MovePipes();

        }

        public readonly List<Bird> Players = new List<Bird>();
        public readonly List<Printable> PrintablePiles = new List<Printable>();


        #region Event
        public static event EventHandler<TicEventArgs> Tic; 

        protected virtual void OnTic()
        {
            EventHandler<TicEventArgs> handler = Tic;
            var e = new TicEventArgs(Players.ToList(), PrintablePiles.ToList(), this);
            handler?.Invoke(this, e);
        }
        #endregion

        #region MainLoop

        #endregion

        public int pipeVelX = -4;
        public List<PipePart> upperPipes;
        public List<PipePart> lowerPipes;
            
        public const int FPS = 30;
        public int FPS_DELAY => Convert.ToInt32( 1000.0 / FPS );

        public const int SCREENWIDTH  = 288;
        public const int SCREENHEIGHT = 512;
        public override int Width => SCREENHEIGHT;
        public override int Height => SCREENHEIGHT;

        public const int PIPEGAPSIZE  = 150; // gap between upper and lower part of pipe
        public static double BASEY => SCREENHEIGHT * 0.79;

        //# image, sound and hitmask  dicts
        //IMAGES, SOUNDS, HITMASKS = {}, {}, {}

        // background
        public int CurrentBackgroundImageIndex = 0;
        public string CurrentBackgroundImage => BACKGROUNDS_LIST[CurrentBackgroundImageIndex];
        public override string Image => CurrentBackgroundImage;
        private Printable printableBackground;
        public Printable PrintableBackground 
        {
            get
            {
                if (printableBackground == null)
                {
                    printableBackground = new Printable( 0, 0, CurrentBackgroundImage );
                }
                return printableBackground;
            }
        }

        private Printable theBase;
        public Printable TheBase
        {
            get
            {
                if (theBase == null)
                {
                    theBase = new Printable( this.basex, Convert.ToInt32( Universe.BASEY), Universe.IMAGES["base"]  );
                }
                return theBase;
            }
        }

        private Printable playAgain;
        public Printable PlayAgain
        {
            get
            {
                if (playAgain == null)
                {
                    playAgain = new Printable( (Universe.SCREENWIDTH - 192)/2 , Universe.SCREENHEIGHT/2,  Universe.IMAGES["pressptoplayagain"]);
                }
                return playAgain;
            }
        }

        private Printable gameOver;
        public Printable GameOver
        {
            get
            {
                if (gameOver == null)
                {
                    gameOver = new Printable( (Universe.SCREENWIDTH - 192)/2 , Universe.SCREENHEIGHT/2,  Universe.IMAGES["gameover"]);
                }
                return gameOver;
            }
        }

        // other
        public int GetPlayerHeight => 24;

        public int GetBaseWidth => 336;

        public int GetBackgroundWidth => 288;

        public int GetPipeHeight => 320;

        public int GetPlayerWidth => 34;

        public int GetPipeWidth => 52;

        public static Dictionary<string, string[]> IMAGESS =new Dictionary<string, string[]>() 
        {
            ["numbers"] = new [] {
                "number-0",
                "number-1",
                "number-2",
                "number-3",
                "number-4",
                "number-5",
                "number-6",
                "number-7",
                "number-8",
                "number-9",
            },            
        };

        public static Dictionary<string, string> IMAGES = new Dictionary<string, string>() 
        {
            ["gameover"] ="gameover",
            ["message"] ="message",
            ["base"] ="base",
            ["pressptoplayagain"] = "pressptoplayagain",
        };

        public static Dictionary<string, string> SOUNDS = new Dictionary<string, string>() 
        {
            ["die"] ="assets/audio/die.ogg",
            ["hit"] ="assets/audio/hit.ogg",
            ["point"] ="assets/audio/point.ogg",
            ["swoosh"] ="assets/audio/swoosh.ogg",
            ["swoosh"] ="assets/audio/wing.ogg",            
        };

        //list of all possible players (tuple of 3 positions of flap)
        public static string[][] PLAYERS_LIST = new []
        {
            // red bird
            new []
            {
                "redbird-upflap",
                "redbird-midflap",
                "redbird-downflap",
            },

            // blue bird
            new []
            {
                "bluebird-upflap",
                "bluebird-midflap",
                "bluebird-downflap",
            },

            // yellow bird
            new []
            {
                "yellowbird-upflap",
                "yellowbird-midflap",
                "yellowbird-downflap",
            },
        };


        // list of backgrounds
        public static string[] BACKGROUNDS_LIST = new []
        {
            "background-day",
            "background-night",
        };

        // list of pipes
        public static string[] PIPES_LIST = new []
        {
            "pipe-green",
            "pipe-red",
        };

        public int GetDigitWidth(int digit)
        {
            return digit == 1 ? 16 : 24;
        }


        public ( List<PipePart>, List<PipePart> ) GetNewPipes()
        {
            var newPipe1 = getRandomPipe();
            var newPipe2 = getRandomPipe();

            newPipe1[0].X = Universe.SCREENWIDTH + 250;
            newPipe1[1].X = Universe.SCREENWIDTH + 250;

            newPipe2[0].X = Universe.SCREENWIDTH + 250 + (Universe.SCREENWIDTH / 2);
            newPipe2[1].X = Universe.SCREENWIDTH + 250 + (Universe.SCREENWIDTH / 2);

            // list of upper pipes
            var upperPipes = new List<PipePart>() { newPipe1[0], newPipe2[0] };
            var lowerPipes = new List<PipePart>() { newPipe1[1], newPipe2[1] };

            return (upperPipes, lowerPipes);
        }

        private void MovePipes()
        {
            // move pipes to left
            for( int i = 0; i< this.upperPipes.Count(); i++ )
            {
                var (uPipe, lPipe) = ( this.upperPipes[i], this.lowerPipes[i]);
                uPipe.X += this.pipeVelX;
                lPipe.X += this.pipeVelX;
            }

            // add new pipe when first pipe is about to touch left of screen
            if ( !upperPipes.Any() || (  0 < upperPipes[0].X &&  upperPipes[0].X < 5 ) )
            {
                var newPipe = getRandomPipe();
                upperPipes.Add(newPipe[0]);
                lowerPipes.Add(newPipe[1]);
            }

            // remove first pipe if its out of the screen
            if (upperPipes[0].X < - this.GetPipeWidth )
            {
                upperPipes.RemoveAt(0);
                lowerPipes.RemoveAt(0);
            }   

            //update pritable pipes
            lock(PrintablePiles)
            {
                PrintablePiles.Clear();
                for( int i = 0; i< this.upperPipes.Count(); i++ )
                {
                    var (uPipe, lPipe) = ( this.upperPipes[i], this.lowerPipes[i]);

                    PrintablePiles.Add(
                        new Printable( uPipe.X, uPipe.Y, Universe.PIPES_LIST[0], -180,null,uPipe.GuidKey )
                    );
                    PrintablePiles.Add(
                        new Printable( lPipe.X, lPipe.Y, Universe.PIPES_LIST[1],null, null, lPipe.GuidKey )
                    );
                }            
            }
        }

        internal void PleaseRestart()
        {

            lock(Players) 
            if (Players.All(p=>p.IsDead)) 
            lock(upperPipes) 
            lock(lowerPipes) 
            (upperPipes, lowerPipes) = GetNewPipes();

        }

        private List<PipePart> getRandomPipe()
        {
            //returns a randomly generated pipe
            // y of gap between upper and lower pipe

            Random random = new Random();
            var gapY = random.Next(0, Convert.ToInt32(Universe.BASEY * 0.6 - Universe.PIPEGAPSIZE) );
            gapY += Convert.ToInt32(Universe.BASEY * 0.2);
            var pipeHeight = this.GetPipeHeight;
            var pipeX = Universe.SCREENWIDTH + 10;
            var pipe = new List<PipePart>() {
                new PipePart() {X = pipeX, Y = gapY - pipeHeight },  // upper pipe
                new PipePart() {X = pipeX, Y = gapY + Universe.PIPEGAPSIZE }, // lower pipe
            };
            return pipe;
        }

    }
}