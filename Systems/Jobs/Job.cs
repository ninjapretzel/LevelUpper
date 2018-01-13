using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelUpper.Systems.Jobs {

	/// <summary> Represents a multi-phase job</summary>
	/// <typeparam name="T"> Input/Output type. </typeparam>
	public class Job<T> {

		public List<Pipe<T>> pipes;

		public Job() {

		}
		



	}
}
