using System;
using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Sample.Target;
using UniversalUnityHooks.Core.FluentInjector;
using UniversalUnityHooks.Core.Models;

namespace UniversalUnityHooks.Sample.Input
{
    public class Core
    {
        [Hook("ExampleType.OutputValue")]
        public static void OutputValue(ExampleType exampleType, ref string value)
        {
            Console.WriteLine($"The output is: {value}");
        }

        [Hook(typeof(ExampleType), "InterceptTest")]
        public static bool InterceptTest(ExampleType exampleType)
        {
            return true; // Blocks rest of method from executing. Returning 'false' would just continue with the method after this hooked method has executed.
        }

        /*
        *
        * /!\ NOTE: Just returning will not work! The following commented out code will **not** work. /!\
        *
        */
        /*
        [Hook(typeof(ExampleType), "InterceptAndReturnTest")]
        public static void InterceptAndReturnTest(ExampleType exampleType, out int returnValue, ref string someString)
        {
            returnValue = 1337; // This will return the following value to the target method, and return said value.
        }
        */

        // These can also be combined as following: 
        // This will return 'returnValue', if the hooked method return value is 'true'.
        [Hook(typeof(ExampleType), "InterceptAndReturnTest")]
        public static bool InterceptAndReturnTest(ExampleType exampleType, out int returnValue, ref string someString)
        {
            if (string.IsNullOrWhiteSpace(someString))
            {
                returnValue = 0;
                return false; // Shorts and will not return anything.
            }
            returnValue = 1337; // This will return the following value to the target method, and return said value.
            return true;
        }

        [Hook(typeof(StaticExampleType), "StaticMethod")]
        public static void StaticTypeStaticMethod(ref string awesomeString)
        {
            awesomeString = "(static type) hijacked your string :)";
        }

        [Hook(typeof(ExampleType), "StaticMethod")]
        public static void StaticMethod(ref string awesomeString)
        {
            awesomeString = "hijacked your string :)";
        }

        [FluentInjectorModule]
        public static void FluentInjectorModule(Injector injector)
        {
            // Changes 'publicInt' from 'public' to 'private'.
            injector.Target<ExampleType>(x => nameof(x.publicInt)).ChangeAccess(false);

            // Changes 'privateInt' from 'private' to 'public'.
            injector.Target<ExampleType>("privateInt").ChangeAccess(true);
            
            // Same code, just a bit differently written.
            injector.Target<ExampleType>(x => nameof(x.publicInt)).To(AccessModifier.Public);

            // Set the target method, which will be used as injection target.
            injector.Target<ExampleType>(x => nameof(x.OutputValue));
            // Set the executing hook, which the method that will get injected if you use 'Hook' or 'Replace'.
            injector.ExecutingHook<Core>(x => nameof(x.TargetHookMethod));
            // Hooks 'ExecutingHook' into 'Target'.
            injector.Hook();
        }

        public static void TargetHookMethod(ExampleType exampleType, ref string value)
        {
            Console.WriteLine("Another target hook method example");
        }
    }
}
