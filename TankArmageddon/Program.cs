﻿using System;

namespace TankArmageddon
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main public class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
#endif
}