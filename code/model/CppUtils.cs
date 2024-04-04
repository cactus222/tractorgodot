using System.Collections.Generic;
// using UnityEngine;
using System.Linq;
using System;

public class CppUtils {

	private static RandomThing randInstance = new RandomThing();

	private class RandomThing {
		Random rand;
		int seed;
		public RandomThing():this(Constants.SEED) {
		}

		public RandomThing(int seed) {
			this.seed = seed;
			rand = new Random(seed);
		}

		public int randInt(int low, int high) {
			// return Random.Range(low, high);
			return rand.Next(low, high);
		}
		public int GetSeed() {
			return this.seed;
		}
	}
	public static void SetSeed(int seed) {
		randInstance = new RandomThing(seed);
	}

	public static void addAll<T>(List<T> list1, List<T> list2) {
		list1.AddRange(list2);
	}

	public static bool contains<T>(List<T> list1, T item) {
		return list1.Contains(item);
	}

	public static bool remove<T>(List<T> list1, T item) {
		return list1.Remove(item);
	}
	

	public static void removeIndex<T>(List<T> list1, int index) {
		list1.RemoveAt(index);
	}

	public static void removeAll<T>(List<T> list1, List<T> list2) {
		foreach (T obj in list2) {
			list1.Remove(obj);
		}
	}

	//low inclusive, high exclusive
	public static int randInt(int low, int high) {
		return randInstance.randInt(low, high);
	}
	public static int GetSeed() {
		return randInstance.GetSeed();
	}
}
