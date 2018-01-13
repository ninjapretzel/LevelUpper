using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LevelUpper.Systems.Jobs {

	/// <summary> Represents a single phase of a job processing some type </summary>
	/// <typeparam name="T"> Input/output type </typeparam>
	public class Pipe<T> : Pipe<T, T> {

		/// <summary> Constructs a Pipe with the given function as the processor </summary>
		/// <param name="processor"> Void func that processes elements flowing through the pipe. </param>
		public Pipe(Action<Block<T, T>> processor) : base(processor) { }

		/// <summary> Constructs a Pipe with the given function as the processor </summary>
		/// <param name="processor"> TOut function that processes elements flowing through the pipe. </param>
		public Pipe(Func<T, T> processor) : base(processor) { }

	}

	/// <summary> Represents a single phase of a <see cref="Job{T}"/> </summary>
	/// <typeparam name="TIn"> Input type into the pipe </typeparam>
	/// <typeparam name="TOut"> Output type from the pipe </typeparam>
	public class Pipe<TIn, TOut> {

		/// <summary> Inputs that have yet to be processed </summary>
		private ConcurrentQueue<Block<TIn, TOut>> inputs;
		
		/// <summary> Blocks that have been pushed through this pipe. </summary>
		private ConcurrentQueue<Block<TIn, TOut>> outputs;

		/// <summary> Assigned straight-through function that processes a job. </summary>
		private Action<Block<TIn, TOut>> processorAction;

		/// <summary> Assigned function which takes a TIn and returns a TOut, used to process elements being piped. </summary>
		private Func<TIn, TOut> processorFunc;

		/// <summary> Task object for running these jobs async. </summary>
		private Task runner;

		/// <summary> Returns true if there are no inputs sitting in the pipe, used to process elements being piped.  </summary>
		public bool Dry { get { return inputs.Count == 0; } }

		/// <summary> Returns true if there are any outputs sitting at the end of the pipe. </summary>
		public bool HasOutput { get { return outputs.Count > 0; } }
		
		/// <summary> Constructs a Pipe with the given function as the processor </summary>
		/// <param name="processor"> Void func that processes elements flowing through the pipe. </param>
		public Pipe(Action<Block<TIn, TOut>> processor) {
			processorAction = processor;
			processorFunc = null;
			inputs = new ConcurrentQueue<Block<TIn, TOut>>();
			outputs = new ConcurrentQueue<Block<TIn, TOut>>();
		}

		/// <summary> Constructs a Pipe with the given function as the processor </summary>
		/// <param name="processor"> TOut function that processes elements flowing through the pipe. </param>
		public Pipe(Func<TIn, TOut> processor) {
			processorAction = null;
			processorFunc = processor;
			inputs = new ConcurrentQueue<Block<TIn, TOut>>();
			outputs = new ConcurrentQueue<Block<TIn, TOut>>();
		}

		/// <summary> Adds an object to process </summary>
		/// <param name="input"> Object to process </param>
		/// <returns> Block holding information for the job </returns>
		public Block<TIn, TOut> Fill(TIn input) {
			Block<TIn, TOut> block = new Block<TIn, TOut>(input);
			inputs.Enqueue(block);
			return block;
		}

		/// <summary> Adds an object to process. Same as <see cref="Fill"/>, but lacks aliteration. </summary>
		/// <param name="t"> Object to process </param>
		/// <returns> Block object representing the request. </returns>
		public Block<TIn, TOut> Enqueue(TIn input) { return Fill(input); }

		/// <summary> Removes a processed object. </summary>
		/// <returns> Block holding information about a completed job </returns>
		public Block<TIn, TOut> Dequeue() {
			Block<TIn, TOut> block;
			if (HasOutput && outputs.TryDequeue(out block)) {
				return block;	
			}
			return null;
		}

		/// <summary> Passes all available output to another pipe. </summary>
		/// <param name="next"> Pipe to pass output to. </param>
		/// <param name="onFail"> Action to call, passing in the </param>
		/// <returns> The <paramref name="next"/> parameter that was passed in. </returns>
		public Pipe<TOut, T> Pass<T>(Pipe<TOut, T> next, Action<Block<TIn, TOut>> onFail = null) {
			while (HasOutput) {
				Block<TIn, TOut> block = Dequeue();

				if (block.success) {
					TOut pass = block.output;
					next.Enqueue(pass);
				} else {
					onFail?.Invoke(block);
				}
				
			}

			return next;
		}

		
		/// <summary> Pulls the next element (if present), and processes it. </summary>
		/// <param name="peek"> Extra function to use to peek at blocks as they are finished, before they are enqueued </param>
		/// <returns> The pipe that this method was called on. </returns>
		public Pipe<TIn, TOut> Flow(Action<Block<TIn, TOut>> peek = null) {
			Block<TIn, TOut> block;
			if (!Dry && inputs.TryDequeue(out block)) {
				block.Start();

				try {
					if (processorAction != null) {
						processorAction(block);
					} else if (processorFunc != null) {
						block.output = processorFunc(block.input);
					}

				} catch (Exception e) {
					block.exception = e;
				}

				block.Finish();
				peek?.Invoke(block);
				outputs.Enqueue(block);
			}
			return this;
		}

		/// <summary> Processes all elements in the pipe (if any are present) </summary>
		/// <param name="peek"> Extra function to use to peek at blocks as they are finished. </param>
		/// <returns> The pipe that this method was called on. </returns>
		public Pipe<TIn, TOut> Flush(Action<Block<TIn, TOut>> peek = null) {
			while (!Dry) { Flow(peek); }
			return this;
		}
		
	}


}
