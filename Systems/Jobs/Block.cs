using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace LevelUpper.Systems.Jobs {

	/// <summary> Convienience type for when inputs and outputs are the same. </summary>
	/// <typeparam name="T"> Generic type of input and output </typeparam>
	public class Block<T> : Block<T, T> {
		/// <inheritdoc />
		public Block() : base() { }
		/// <inheritdoc />
		public Block(Block<T, T> other) : base(other) { }
		/// <inheritdoc />
		public Block(T input) : base(input) { }

	}

	/// <summary> Represents a part of a job with defined input and output types. </summary>
	/// <typeparam name="TIn"> Type of input </typeparam>
	/// <typeparam name="TOut"> Type of output </typeparam>
	public class Block<TIn, TOut> {
		/// <summary> Initial object passed to Fill() or Enqueue() </summary>
		public TIn input;
		/// <summary> Output object assigned by a processor function. </summary>
		public TOut output;

		/// <summary> Tag assigned to this block by the tagger function </summary>
		public string tag;

		/// <summary> Has the work for this block been completed? Don't set this, only check it </summary>
		/// <remarks> Cannot be made into a property, unfortunately, as that would enable the ""optimizations"" 'volatile' disables </remarks>
		public volatile bool done;

		/// <summary> Returns true if done, and no exception was generated </summary>
		public bool success { get { return done && exception == null; } }

		/// <summary> Holds exception, if one got generated when running the task. </summary>
		public Exception exception;

		/// <summary> Time of Job Block creation </summary>
		public DateTime created;
		/// <summary> Time of Job Block began at </summary>
		public DateTime started;
		/// <summary> Time JobBlock finished at </summary>
		public DateTime finished;

		/// <summary> Gets the number of milliseconds it took this job to run. Returns -1 if the job is not yet finished. </summary>
		public long Duration { get { return done ? (long)(finished - started).TotalMilliseconds : -1; } }

		/// <summary> Gets the number of milliseconds it took before this job was fufilled. </summary>
		public long FullDuration { get { return done ? (long)(finished - created).TotalMilliseconds : -1; } }

		/// <summary> Creates an empty block. Assigns nothing. </summary>
		public Block() { }

		/// <summary> Creates a copy of <paramref name="other"/> Used as convinience for conversion to <see cref="Block{T}"/> </summary>
		/// <param name="other"> Block to copy. </param>
		public Block(Block<TIn, TOut> other) {
			input = other.input;
			output = other.output;
			tag = other.tag;
			done = other.done;
			exception = other.exception;
			created = other.created;
			started = other.started;
			finished = other.finished;
		}

		/// <summary> Basic constructor for a block item. </summary>
		/// <param name="input"></param>
		public Block(TIn input) {
			this.input = input;
			output = default(TOut);

			done = false;
			tag = null;
			exception = null;

			created = DateTime.UtcNow;
			started = DateTime.MinValue;
			finished = DateTime.MinValue;
		}

		/// <summary> Siginals to this block that it has finished processing </summary>
		public void Finish() {
			finished = DateTime.UtcNow;
			done = true;
		}

		/// <summary> Siginals to this block that it has began processing. </summary>
		public void Start() {
			started = DateTime.UtcNow;
		}
	}

}
