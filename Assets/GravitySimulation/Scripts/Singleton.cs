// Copyright (c) 2019 Subo Lee (KAIST HAJE)
// Please direct any bugs/comments/suggestions to suboo0308@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace SansyHuman
{
    /// <summary>
    /// Base class of all singleton.
    /// </summary>
    /// <typeparam name="T">Singleton class</typeparam>
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>Instance of the singleton.</summary>
        protected static T instance;

        /// <value>Gets the instance of the singleton.</value>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        instance = new GameObject("@" + typeof(T).ToString(),
                                                   typeof(T)).GetComponent<T>();
                        DontDestroyOnLoad(instance);
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes the singleton.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
                instance = FindObjectOfType<T>();

            DontDestroyOnLoad(instance);
        }
    }
}