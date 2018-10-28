using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    static class ExtensionMethods
	{
        /// <summary>
        /// Naive IndexOfAny method for StringBuilders
        /// </summary>
        public static int IndexOfAny(this StringBuilder haystack, char[] toFind, int startIndex)
        {
            if (toFind.Length == 0)
                return 0;

            for (int i = startIndex; i < haystack.Length; i++)
            {
                char c = haystack[i];

                foreach (var ch in toFind)
                {
                    if (c == ch)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the start of the contents in a StringBuilder
        /// </summary>        
        /// <param name="value">The string to find</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder haystack, string value, int startIndex, int count, bool ignoreCase)
        {
            int index;
            int length = value.Length;
            int maxSearchLength = startIndex + (count - length) + 1;

            if (ignoreCase)
            {
                var firstSearchedCharacter = Char.ToLower(value[0]);

                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(haystack[i]) == firstSearchedCharacter)
                    {
                        index = 1;
                        while ((index < length) && (Char.ToLower(haystack[i + index]) == Char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }
            else
            {
                var firstSearchedCharacter = value[0];

                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (haystack[i] == firstSearchedCharacter)
                    {
                        index = 1;
                        while ((index < length) && (haystack[i + index] == value[index]))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }
        }
    }
}
