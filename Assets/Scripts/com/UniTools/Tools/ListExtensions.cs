﻿/*
 * Created by Alexander Sosnovskiy. May 3, 2016
 */
using System;
using System.Collections.Generic;

namespace UniTools
{
	public static class ListExtensions
	{
		public static List<T> Clone<T>(this List<T> list)
		{
			return new List<T>(list);
		}

		public static void Reshuffle<T>(this List<T> list)
		{
			for (int t = 0; t < list.Count; t++)
			{
				var tmp = list[t];

				int r = UnityEngine.Random.Range(t, list.Count);

				list[t] = list[r];

				list[r] = tmp;
			}
		}

		/*public static T[] ForEach<T>(this T[] array, Action<T> callback)
		{
			if (callback != null)
				for (int i = 0; i < array.Length; i++)
				{
					callback(array[i]);
				}

			return array;
		}
        */
	    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action) {

	        if (action != null) {
	            foreach (T instance in source) {
	                action(instance);
	            }
	        }

	        return source;
	    }

    }
}