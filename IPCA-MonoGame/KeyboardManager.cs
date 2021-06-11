using System;
using System.Collections.Generic;  // Dictionary and List
using System.Linq;  // Collections' functions (Except, ...
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IPCA.MonoGame
{ /*
   *  MonoGame - Default: 
   *    Keyboard.IsDown(tecla) -> bool 
   *    Keyboard.IsUp(tecla) -> bool
   *
   *  IPCA.KeyboardManager 
   *     Keyboard.IsKeyDown(tecla) -> bool
   *     Keyboard.IsKeyUp(tecla)   -> bool
   *     Keyboard.GoingDown(tecla) -> bool
   *     Keyboard.GoingUp(tecla)   -> bool
   */

    public enum KeysState
    {
        Up,
        Down,
        GoingUp,
        GoingDown
    }
    
    public class KeyboardManager : GameComponent
    {
        // Variáveis de classe (suporte ao Singleton)
        private static KeyboardManager _instance; // Referencia à única instância de KeyboardManager
        // Variáveis de instância
        private Dictionary<Keys, KeysState> _keyboardState;
        /*
                Keys.A => {
                             KeysState.GoingUp => [ Action1, Action2 ]
                             KeysState.Down => [ Action1, Action2 ]
                          }
                Keys.B => { 
                            KeysState.Up => [ Action1 ]
                            KeysState.GoingUp => [ Action2 ]
                          }
         */
        private Dictionary<Keys, Dictionary<KeysState, List<Action>>> _actions;
        
        public KeyboardManager(Game game) : base(game)
        {
            // Validar que Singleton ainda não foi criado.
            if (_instance != null) throw new Exception("KeyboardManager constructor called twice");
            _instance = this; // guardar a única instância no singleton.
            
            _keyboardState = new Dictionary<Keys, KeysState>();
            _actions = new Dictionary<Keys, Dictionary<KeysState, List<Action>>>();
            game.Components.Add(this);   // "auto instalável"
        }

        // Action é uma referência a uma função     void f(void)
        public static void Register(Keys key, KeysState state, Action code)
        {
            // Do we have this key already in the dictionary?
            if (!_instance._actions.ContainsKey(key)) 
                _instance._actions[key] = new Dictionary<KeysState, List<Action>>();
            
            // For this key, do we have that state created?
            if (!_instance._actions[key].ContainsKey(state))
                _instance._actions[key][state] = new List<Action>();
            
            // Add the code to the key/state pair
            _instance._actions[key][state].Add(code);
            // Add the key to the keyboard state dictionary
            _instance._keyboardState[key] = KeysState.Up;
        }

        public static bool IsKeyDown(Keys k) => 
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.Down;
        
        public static bool IsKeyUp(Keys k) => 
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.Up;
        public static bool IsGoingDown(Keys k) => 
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.GoingDown;
        public static bool IsGoingUp(Keys k) => 
            _instance._keyboardState.ContainsKey(k) && _instance._keyboardState[k] == KeysState.GoingUp;

        
        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            List<Keys> pressedKeys = state.GetPressedKeys().ToList();
            
            // Process pressed keys
            foreach (Keys key in pressedKeys)
            {
                // If we didn't know anything about this key, then probably it was up.
                if (!_keyboardState.ContainsKey(key)) _keyboardState[key] = KeysState.Up;

                // What was the previous state, and decide what is our next state
                switch (_keyboardState[key])
                {
                   /*   Estado Anterior  Agora   Guardo
                    *      DOWN           DOWN    Down
                    *    GOING DOWN       DOWN    Down
                    *       UP            DOWN    Going Down
                    *    GOING UP         DOWN    Going Down
                    */
                   case KeysState.Down: 
                   case KeysState.GoingDown:
                       _keyboardState[key] = KeysState.Down;
                       break;
                   case KeysState.Up:
                   case KeysState.GoingUp:
                       _keyboardState[key] = KeysState.GoingDown;
                       break;
                }
            }
            // Processed released keys
            //   Keys[] x = _keyboardState.Keys.Except(pressedKeys).ToArray();
            //   foreach (Keys key in x)
            // same as...
            foreach (Keys key in _keyboardState.Keys.Except(pressedKeys).ToArray())
            {
                /*   Estado Anterior  Agora   Guardo
                 *      DOWN           UP      GoingUp
                 *    GOING DOWN       UP      GoingUp
                 *       UP            UP      UP
                 *    GOING UP         UP      UP
                 */
                switch (_keyboardState[key])
                {
                    case KeysState.Down:
                    case KeysState.GoingDown:
                        _keyboardState[key] = KeysState.GoingUp;
                        break;
                    case KeysState.Up:
                    case KeysState.GoingUp:
                        _keyboardState[key] = KeysState.Up;
                        break;
                }
            }
            
            // Invocar as funções registadas!
            foreach (Keys key in _actions.Keys)
            {
                KeysState kstate = _keyboardState[key];
                if (_actions[key].ContainsKey(kstate))
                {
                    foreach (Action action in _actions[key][kstate])
                    {
                        action();
                    }
                }
            }
        }
    }
}