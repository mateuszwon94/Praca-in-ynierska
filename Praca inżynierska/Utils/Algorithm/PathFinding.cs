using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using System.Threading;
using System.Threading.Tasks;

namespace PracaInzynierska.Utils.Algorithm {
	public static partial class PathFinding {

        /// <summary>
        /// Algorytm A* znajdowania najkrotszej sciezki na planszy.
        /// </summary>
        /// <param name="from">Pole startowe</param>
        /// <param name="to">Pole, ktore chcemy osiagnac</param>
        /// <param name="heuristic">Metryka w jakiej maja byc szacowana odleglosc do pola docelowego</param>
        /// <returns>Lista pol skladajaca sie na sciezke</returns>
		public static IList<MapField> AStar(MapField from, MapField to, Metric.HeuristicFunc heuristic) {
		    if ( (from == null) || (to == null) ) throw new NullReferenceException();
			if ( !to.IsAvaliable ) throw new FieldNotAvaliableException();

            //Lita pol do przeszukania
			List<PathFindingNode> openList = new List<PathFindingNode> {
												 new PathFindingNode(from, null, 0, Metric.EuclideanDistance(from, to))
											 };
            //Lista przeszukanych pol
			List<PathFindingNode> closeList = new List<PathFindingNode>();

			while ( openList.Count != 0 ) {
				PathFindingNode current = openList.RemoveAndGet(0);

			    if ( current.This == to ) { return ReconstructPath(current); }

			    closeList.Add(current);

				Parallel.ForEach(current.This.Neighbour, neighbour => {
															 if ( !neighbour.IsAvaliable || closeList.Contains(neighbour) ) return;

															 PathFindingNode neighbourNode = new PathFindingNode(neighbour, current,
																												 current.CostFromStart + Metric.EuclideanDistance(current.This, neighbour),
																												 heuristic(neighbour, to));

															 //Zmiana parametrow sciezki jesli droga do sasiada jest krotsza z obecnego pola niz ustalona wczesniej
															 if ( openList.Contains(neighbourNode) ) {
																 PathFindingNode oldNode = openList[openList.IndexOf(neighbourNode)];
																 if ( neighbourNode.CostFromStart < oldNode.CostFromStart ) {
																	 oldNode.CostFromStart = neighbourNode.CostFromStart;
																	 oldNode.CostToEnd = neighbourNode.CostToEnd;
																 }
															 } else {
																 lock ( openList ) { openList.Add(neighbourNode); }
															 }
														 });

				//openList.AsParallel().WithDegreeOfParallelism(4).OrderBy(field => field.AllCost);
				openList.Sort();
			}
			
			throw new FieldNotAvaliableException();
		}

		/// <summary>
		/// Funkcja rekonstruująca sciezke przejscia.
		/// </summary>
		/// <param name="node">Koncowy wezel sciezki, z ktorego nalezy dokonac rekonstrukcji</param>
		/// <returns></returns>
		private static IList<MapField> ReconstructPath(PathFindingNode node) {
			List<MapField> path = new List<MapField>();

		    while ( true ) {
		        path.Add(node.This);
		        if ( node.Parent == null ) break;
                node = node.Parent;
		    }

		    path.Reverse();
			return path;
		}

		public static T RemoveAndGet<T>(this IList<T> list, int index) {
			lock ( list ) {
				T value = list[index];
				list.RemoveAt(index);
				return value;
			}
		}

		#region Private

		private class PathFindingNode : IComparable<PathFindingNode> {
			internal PathFindingNode(MapField This, PathFindingNode Parent, float costFromStart, float costToEnd) {
				this.This = This;
				this.Parent = Parent;

				CostFromStart = costFromStart;
				CostToEnd = costToEnd;
			}

			public MapField This { get; }
			public PathFindingNode Parent { get; set; }

			public float CostFromStart { get; set; }
			public float CostToEnd { get; set; }

			public float AllCost => CostFromStart + CostToEnd;

			public static bool operator ==(PathFindingNode one, PathFindingNode two) => one?.This == two?.This;
			public static bool operator !=(PathFindingNode one, PathFindingNode two) => !(one == two);

			public static bool operator <(PathFindingNode one, PathFindingNode two) => one.AllCost < two.AllCost;
			public static bool operator >(PathFindingNode one, PathFindingNode two) => one.AllCost > two.AllCost;


			public bool Equals(PathFindingNode other) {
				return This == other.This;
			}

			public int CompareTo(PathFindingNode other) {
				if ( this < other ) return -1;
				else if ( this == other ) return 0;
				else return 1;
			}

			public override bool Equals(object obj) {
				if ( ReferenceEquals(null, obj) ) return false;
				if ( ReferenceEquals(this, obj) ) return true;
				if ( obj.GetType() != this.GetType() ) return false;

				return Equals((PathFindingNode)obj);
			}

			public override int GetHashCode() {
				return This.GetHashCode();
			}
		}
		
		private static bool Contains(this IEnumerable<PathFindingNode> list, MapField field) {
			return list.Any(val => val == new PathFindingNode(field, null, 0, 0));
		} 

		#endregion

	}
}