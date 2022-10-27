using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Hokm.Properties;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using static Hokm.GameClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Hokm.GUI
{
    internal class AnimationHandler
    {
        private GameClient g;
        private int delay;

        public AnimationHandler(GameClient gClient, int delay = 1)
        {
            ///<summary>
            /// Initializing the class
            /// <param name="gClient">The GameClient itself</param>
            /// <param name="delay">The intervals between each animation in milliseconds</param>
            /// <return> None </return>
            ///</summary>
            
            this.g = gClient;
            this.delay = delay;
        }


        public void AnimateCard(int[] dest, Control control)
        {
            ///<summary>
            /// Animating the card to the desired location
            /// <param name="dest">Array of {x,y} of the destination </param>
            /// <param name="control">The Control the card is on</param>
            /// <return> None </return>
            ///</summary>

            int jmp = 1;

            // Initializing the timer
            System.Windows.Forms.Timer timerA = new System.Windows.Forms.Timer();
            timerA.Interval = delay;
            timerA.Tick += new EventHandler(TickAnim);
            timerA.Start();

            void TickAnim(object sender, EventArgs e)
            {
                ///<summary>
                /// The function moves the card by 'jmp' value each call until reached the destination
                /// <return> None </return>
                ///</summary>
                int times = 0;
                int directionX = -1 * jmp;
                int directionY = -1 * jmp;

                if (dest[0] >= control.Left)
                    directionX = jmp;
                if (dest[1] >= control.Top)
                    directionY = jmp;

                while (control.Left != dest[0] || control.Top != dest[1])
                {
                    if (control.Left != dest[0])
                    {
                        this.g.Invoke((Action)delegate ()
                        {
                            control.Left += directionX;
                        });
                    }
                    if (control.Top != dest[1])
                    {
                        this.g.Invoke((Action)delegate ()
                        {
                            control.Top += directionY;
                        });
                    }
                    // In order to avoid awful visual bugs, the Control updates itself each 15 iterations
                    if (times % 15 == 0)
                    {
                        control.Update();
                        g.Update();
                    }

                    if (control.Left == dest[0] && control.Top == dest[1])
                        timerA.Stop();
                    times++;
                }
            }
        }

    }
}
