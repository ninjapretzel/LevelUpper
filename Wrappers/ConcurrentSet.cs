using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

/// <summary> Wrapper for <see cref="ConcurrentDictionary{TKey, TValue}"/> that acts like a Set. </summary>
/// <typeparam name="T"> Generic Type </typeparam>
public class ConcurrentSet<T> : IEnumerable<T> {
	
	/// <summary> Content of the Set. All we care about are the keys. </summary>
	private ConcurrentDictionary<T, int> content;

	/// <summary> Location storing the interlocked size. </summary>
	private int count = 0;

	/// <summary> Fast accessor for the size of the Set </summary>
	public int Count { get { return count; } }

	/// <summary> Basic constructor </summary>
	public ConcurrentSet() {
		content = new ConcurrentDictionary<T, int>();
	}

	/// <summary> Wrap through to the content IsEmpty </summary>
	public bool IsEmpty { get { return content.IsEmpty; } }

	/// <summary> See if the Set contains <paramref name="val"/> </summary>
	/// <param name="val"> Value to check for </param>
	/// <returns> true if <paramref name="val"/> was in the set when it peeked, false otherwise. </returns>
	public bool Has(T val) { return content.ContainsKey(val); }

	/// <summary> See if the Set contains <paramref name="val"/> </summary>
	/// <param name="val"> Value to check for </param>
	/// <returns> true if <paramref name="val"/> was in the set when it peeked, false otherwise. </returns>
	public bool Contains(T val) { return content.ContainsKey(val); }

	/// <summary> Adds/sets <paramref name="val"/> as a part of the Set </summary>
	/// <param name="val"></param>
	public bool Add(T val) {
		var had = content.ContainsKey(val);
		content[val] = 1;
		if (!had) { Interlocked.Increment(ref count); }
		return had;
	}

	/// <summary> Removes <paramref name="val"/> from the set </summary>
	/// <param name="val"></param>
	public void Remove(T val) {
		int __;
		if (content.TryRemove(val, out __)) {
			Interlocked.Decrement(ref count);
		}
	}

	/// <summary> Removes all elements from the set that satisfy a predicate. </summary>
	/// <param name="predicate"> Predicate function to use to select things to remove. </param>
	public void RemoveWhere(Func<T, bool> predicate) {
		// Avoids allocation if not neccessary. 
		List<T> rem = null;

		foreach (var item in this) {
			if (predicate(item)) { 
				( rem ?? (rem = new List<T>()) ).Add(item); 
			}
		}

		if (rem != null) {
			foreach (var item in rem) { Remove(item); }
		}
	}

	/// <summary> Removes all values from the set. </summary>
	/// <remarks> Drops the <see cref="content"/> Dictionary backing it, and Interlocks an exchange of <see cref="count"/> to zero</remarks>
	public void Clear() {
		if (count > 0) {
			content = new ConcurrentDictionary<T, int>();
			Interlocked.Exchange(ref count, 0);
		}
	}

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator() { return content.Keys.GetEnumerator(); }

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() { return content.Keys.GetEnumerator(); }
}
