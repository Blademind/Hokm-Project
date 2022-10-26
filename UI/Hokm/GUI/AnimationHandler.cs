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

        public AnimationHandler(GameClient g, int delay = 1)
        {
            this.g = g;
            this.delay = delay;

        }


        public void AnimateCard(int[] dest, Control control)
        {
            int t = 1;

            System.Windows.Forms.Timer timerA = new System.Windows.Forms.Timer();
            timerA.Interval = delay;
            timerA.Tick += new EventHandler(TickAnim);
            timerA.Start();

            void TickAnim(object sender, EventArgs e)
            {
                int times = 0;
                int directionX = -1 * t;
                int directionY = -1 * t;

                if (dest[0] >= control.Left)
                    directionX = t;
                if (dest[1] >= control.Top)
                    directionY = t;

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


            #region trash
            /*
            new Task(() =>
            {
                int directionX = -1 * t;
                int directionY = -1 * t;

                if (dest[0] >= control.Left)
                    directionX = t;
                if (dest[1] >= control.Top)
                    directionY = t;

                while (control.Left != dest[0] || control.Top != dest[1])
                {
                    try
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
                        control.Refresh();
                        //Thread.Sleep(this.delay);
                    }
                    catch
                    {
                        // form could be disposed
                        break;
                    }
                }


            }).Start();*/
            #endregion
        }

    }
}
