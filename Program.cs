using System;
using System.Threading;

namespace Rover
{

    class Program
    {
        //Create AutoResetEvent to control state of the threads
        private static AutoResetEvent detectingEvent = new AutoResetEvent(false);
        private static AutoResetEvent drillingEvent = new AutoResetEvent(false);
        private static AutoResetEvent cleaningEvent = new AutoResetEvent(false);

        //To generate conditional output
        private static bool detect = true;
        private static bool drill = false;
        private static bool clean = false;

        private static int position = 0;
        private static int found = 0;

        static void Main(string[] args)
        {
            Thread detecting = new Thread(Detecting);
            //Start detecting thread
            detecting.Start();

            Thread drilling = new Thread(Drilling);
            //Start drilling thread
            drilling.Start();

            Thread cleaning = new Thread(Cleaning);
            //Start cleaning thread
            cleaning.Start();

            //Perform action when 'CTRL + C' is pressed
            Console.CancelKeyPress += delegate {
                Console.Clear();
                //Indicate the main thread is stopped
                Console.WriteLine("The rover is stopped.");
                Console.WriteLine("The total material found : {0}", found);
            };


        }

        static void Detecting()
        {
            //First time start the rover
            generateOutput();
            Console.WriteLine("The rover start now...");
            detectMaterial();
            while (true)
            {
                //Wait for the detectingEvent to be signaled
                //The detectingEvent is signaled after the material is detected
                detectingEvent.WaitOne();

                //Indicate the detectingEvent is waiting now
                detect = false;
                generateOutput();
                Console.WriteLine("The material is detected at position: {0}.", position);
                Console.WriteLine("The drilling event will start soon...");
                Thread.Sleep(2000);

                //Signal the drillingEvent
                drillingEvent.Set();
            }
        }

        static void Drilling()
        {
            while (true)
            {
                //Wait for the drillingEvent 
                drillingEvent.WaitOne();
                
                //Action for drillingEvent

                //Indicate the drillingEvent start now
                drill = true;
                generateOutput();
                Console.WriteLine("The drilling event is running now...");
                Console.WriteLine("The cleaning event will start after the drilling event.");
                //Simulate the drillingEvent
                Thread.Sleep(2000);
                //Indicate the drillingEvent end
                drill = false;

                //Signal the cleaningEvent
                cleaningEvent.Set();
            }
        }

        static void Cleaning()
        {
            while (true)
            {
                cleaningEvent.WaitOne();

                //Action for cleaningEvent

                //Indicate the cleaningEvent start now
                clean = true;
                //Indicate the detect event start simultaneously
                detect = true;
                generateOutput();
                //Simulate the cleaningEvent
                Console.WriteLine("The cleaning event is processing now...");
                Console.WriteLine("The detecting event is processing now...");
                Thread.Sleep(2000);
                //Indicate the cleaningEvent end
                clean = false;
                generateOutput();
                Console.WriteLine("The cleaning event is finished.");
                Console.WriteLine("The detecting event is processing now...");
        
                //Run detect material process
                detectMaterial();
            }
        }

        static void detectMaterial()
        {
            Random rnd = new Random();
            int rndNumber = rnd.Next();
            //Simulate the detecting process
            while (rndNumber % 10 != 0)
            {
                rndNumber = rnd.Next();
            }
            found++;
            position = rndNumber % 100 + rnd.Next() % 10;
            Thread.Sleep(2000);
            //Signal the detectingEvent
            detectingEvent.Set();
            return;
        }

        static void generateOutput()
        {
            Console.Clear();
            if (detect == true)
            {
                Console.WriteLine("Detecting(): detecting...");
            }
            else
            {
                Console.WriteLine("Detecting(): waiting...");
            }
            if (drill == true)
            {
                Console.WriteLine("Drilling(): drilling...");
            }
            else
            {
                Console.WriteLine("Drilling(): waiting...");
            }
            if (clean == true)
            {
                Console.WriteLine("Cleaning(): cleaning...");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Cleaning(): waiting...");
                Console.WriteLine();
            }
        }
    }
}
