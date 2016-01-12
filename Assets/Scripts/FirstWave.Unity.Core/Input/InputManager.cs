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

        public bool KeyReleased(string key)
        {
            return (prevState.ContainsKey(key) && prevState[key]) &&
                   (currentState.ContainsKey(key) && !currentState[key]);
        }

        public bool KeyPressed(string key)
        {
            return (prevState.ContainsKey(key) && !prevState[key]) &&
                   (currentState.ContainsKey(key) && currentState[key]);
        }

        public bool KeyDown(string key)
        {
            return currentState.ContainsKey(key) && currentState[key];
        }

        public void Flush()
        {
            prevState.Clear();
            currentState.Clear();
        }

        public void FlushKey(string key)
        {
            if (prevState.ContainsKey(key))
                prevState.Remove(key);

            if (currentState.ContainsKey(key))
                currentState.Remove(key);
        }
    }
}
