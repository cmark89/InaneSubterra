using System;

namespace InaneSubterra
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (InaneSubterra game = new InaneSubterra())
            {
                game.Run();
            }
        }
    }
#endif
}

