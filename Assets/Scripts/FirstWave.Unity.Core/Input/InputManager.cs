using FirstWave.Unity.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FirstWave.Unity.Core.Input
{
    public class InputManager : SafeSingleton<InputManager>
    {
        public const string UP = "up";
        public const string DOWN = "down";
        public const string LEFT = "left";
        public const string RIGHT = "right";

        public string[] KeysToMap;

        public string[] allKeys
        {
            get
            {
                return (KeysToMap ?? new string[0]).Union(new[] { UP, DOWN, LEFT, RIGHT }).ToArray();
            }
        }

        private IDictionary<string, bool> prevState;
        private IDictionary<string, bool> currentState;

        protected override string managerName
        {
            get { return "Input Manager"; }
        }

        void Awake()
        {
            prevState = new Dictionary<string, bool>();
            currentState = new Dictionary<string, bool>();               
        }

        void Update()
        {
            foreach (var key in currentState.Keys)
                prevState[key] = currentState[key];

            currentState.Clear();

            var userMappedKeys = KeysToMap ?? new string[0];

            // Now handle each key we care about
            for (int i = 0; i < userMappedKeys.Length; i++)
            {
                var key = userMappedKeys[i];

                bool state = UnityEngine.Input.GetButton(key);

                currentState.Add(key, state);
            }

            var horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");

            currentState.Add(LEFT, horizontal < 0);
            currentState.Add(RIGHT, horizontal > 0);

            var vertical = UnityEngine.Input.GetAxisRaw("Vertical");

            currentState.Add(UP, vertical > 0);
            currentState.Add(DOWN, vertical < 0);  
        }

        public static bool KeyReleased(string key)
        {
            var im = Instance;

            return (im.prevState.ContainsKey(key) && im.prevState[key]) &&
                   (im.currentState.ContainsKey(key) && !im.currentState[key]);
        }

        public static bool KeyPressed(string key)
        {
            var im = Instance;

            return (im.prevState.ContainsKey(key) && !im.prevState[key]) &&
                   (im.currentState.ContainsKey(key) && im.currentState[key]);
        }

        public static bool KeyDown(string key)
        {
            var im = Instance;

            return im.currentState.ContainsKey(key) && im.currentState[key];
        }

        public static void Flush()
        {
            Instance.prevState.Clear();
            Instance.currentState.Clear();
        }

        public static void FlushKey(string key)
        {
            var im = Instance;

            if (im.prevState.ContainsKey(key))
                im.prevState.Remove(key);

            if (im.currentState.ContainsKey(key))
                im.currentState.Remove(key);
        }
    }
}
