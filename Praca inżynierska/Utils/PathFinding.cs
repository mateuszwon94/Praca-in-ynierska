using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using static PracaInzynierska.Utils.PathFinding.Metric;

namespace PracaInzynierska.Utils {
	public static partial class PathFinding {
		public static IList<MapField> AStar(MapField from, MapField to, HeuristicFunc heuristic) {
			if ( !to.IsAvaliable ) throw new FieldNotAvaliableException();

			List<PathFindingNode> openList = new List<PathFindingNode> {
												 new PathFindingNode(from, null, 0, heuristic(from, to))
											 };

			List<PathFindingNode> closeList = new List<PathFindingNode>();

			while ( openList.Count != 0 ) {
				PathFindingNode current = openList.RemoveAndGet(0);

				if ( current.This == to ) {
					return ReconstructPath(current);
				}

				closeList.Add(current);
				foreach ( MapField neighbour in current.This.Neighbour ) {
					if ( !neighbour.IsAvaliable ) continue;

					PathFindingNode neighbourNode = new PathFindingNode(neighbour, current,
																		current.CostFromStart + EuclideanDistance(current.This, neighbour), 
																		heuristic(neighbour, to));

					PathFindingNode oldNode = openList.FirstOrDefault(node => node == neighbourNode);

					if ( (oldNode != null) && (neighbourNode.CostFromStart < oldNode.CostFromStart) ) {
						oldNode.CostFromStart = neighbourNode.CostFromStart;
						oldNode.CostToEnd = neighbourNode.CostToEnd;
					} else if ( oldNode == null ) {
						openList.Add(neighbourNode);
					}
				}

				openList.Sort();
			}
			
			throw new FieldNotAvaliableException();
		}

		private static IList<MapField> ReconstructPath(PathFindingNode node) {
			List<MapField> path = new List<MapField>();

			while ( node.Parent != null ) {
				path.Add(node.This);
				node = node.Parent;
			}
			path.Add(node.This);

			path.Reverse();
			return path;
		}

		private class PathFindingNode : IComparable<PathFindingNode> {
			internal PathFindingNode(MapField This, PathFindingNode parrent, double costFromStart, double costToEnd) {
				this.This = This;
				Parent = parrent;

				CostFromStart = costFromStart;
				CostToEnd = costToEnd;
			}

			public MapField This { get; }
			public PathFindingNode Parent { get; set; }

			public double CostFromStart { get; set; }
			public double CostToEnd { get; set; }

			public double AllCost => CostFromStart + CostToEnd;

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

		/// <summary>
		/// Removes items from list and returns it
		/// </summary>
		private static T RemoveAndGet<T>(this IList<T> list, int index) {
			lock ( list ) {
				T value = list[index];
				list.RemoveAt(index);
				return value;
			}
		}
	}
}