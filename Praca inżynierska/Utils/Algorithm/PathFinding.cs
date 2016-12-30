using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using System.Threading.Tasks;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;

namespace PracaInzynierska.Utils.Algorithm {

	public static partial class PathFinding {
		/// <summary>
		/// Algorytm A* znajdowania najkrotszej sciezki na planszy.
		/// </summary>
		/// <param name="from">Pole startowe</param>
		/// <param name="to">Pole, ktore chcemy osiagnac</param>
		/// <param name="heuristic">Metryka w jakiej maja byc szacowana odleglosc do pola docelowego</param>
		/// <param name="forbidenFields">Pola przez które nie wolno przejść przy danym poszukiwaniu ścieżki</param>
		/// <returns>Lista pol skladajaca sie na sciezke</returns>
		public static IList<MapField> AStar(MapField from, MapField to, heuristicFunc heuristic, params MapField[] forbidenFields) {
		    if ( (from == null) || (to == null) ) throw new NullReferenceException();			                     // ktores z pol nie istnieje
			if ( !to.IsAvaliable ) throw new FieldNotAvaliableException();					            	         // pole jest niedostepne
			if ( from.Neighbour.Contains(to) ) return new List<MapField>(2) { from, to, };                           // pola from i to leza kolo siebie

			//Lita pol do przeszukania
	        List<PathFindingNode> openList = new List<PathFindingNode> {
												 new PathFindingNode(from)
											 };
            //Lista przeszukanych pol
			List<PathFindingNode> closeList = new List<PathFindingNode>(forbidenFields.Select(forbidenField => new PathFindingNode(forbidenField)));

			bool reachedGoal = false;

			//dopoki jakiekolwiek pole moze zostac jeszcze przebadane
	        while ( openList.Count != 0 ) {
				PathFindingNode current = openList.RemoveAtAndGet(0);
				closeList.Add(current);


		        if ( heuristic == NullDistance && current.This == to ) {
			        return ReconstructPath(current);
		        } else if ( current.This == to ) { // znaleziono pole do ktorego dazylismy
			        reachedGoal = true;
		        } else if ( reachedGoal ) {
			        PathFindingNode endNode = openList.Concat(closeList)
													  .First(node => node.This == to);

			        float minValInOpenList = openList.Select(node => node.CostFromStart)
													 .Min();

			        if ( minValInOpenList < endNode.CostFromStart ) return ReconstructPath(endNode);
		        }

		        foreach ( PathFindingNode neighbourNode in current.This.Neighbour.Where(neighbour => neighbour.IsAvaliable)
																  .Select(neighbour => new PathFindingNode(neighbour) {
																														  Parent = current,
																														  CostFromStart = current.CostFromStart +
																																		  EuclideanDistance(current.This, neighbour) * current.This.Cost,
																														  CostToEnd = heuristic(neighbour, to)
																													  }) ) {
			        //Zmiana parametrow sciezki jesli droga do sasiada jest krotsza z obecnego pola niz ustalona wczesniej
			        if ( heuristic != NullDistance && closeList.Contains(neighbourNode) ) {
				        PathFindingNode oldNode = closeList[closeList.IndexOf(neighbourNode)];
				        if ( neighbourNode.CostFromStart < oldNode.CostFromStart ) {
					        closeList.Remove(oldNode);
					        openList.Add(neighbourNode);
				        }
			        } else if ( openList.Contains(neighbourNode) ) {
				        PathFindingNode oldNode = openList[openList.IndexOf(neighbourNode)];
				        if ( neighbourNode.CostFromStart < oldNode.CostFromStart ) {
					        oldNode.CostFromStart = neighbourNode.CostFromStart;
					        oldNode.Parent = neighbourNode.Parent;
				        }
			        } else { openList.Add(neighbourNode); }
		        }

		        //openList.AsParallel().WithDegreeOfParallelism(4).OrderBy(field => field.AllCost);
				openList.Sort();
			}

			//jesli niedotarto do zadanego celu
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
			    if ( node.Parent == null ) { break; }
				node = node.Parent;
		    }

		    path.Reverse();
			return path;
		}

		/// <summary>
		/// Funkcja rozszerzajaca. Usuwa z listy element na zadanej pozycji i zwraca go
		/// </summary>
		/// <typeparam name="T">Typ jakim sparametryzowana jest lista</typeparam>
		/// <param name="list">Lista z ktorej chcemy uunac dany element</param>
		/// <param name="index">Indeks spod jakiego chcemy usunac dany element</param>
		/// <returns>Usuniety element z listy</returns>
		public static T RemoveAtAndGet<T>(this IList<T> list, int index) {
			lock ( list ) {
				T value = list[index];
				list.RemoveAt(index);
				return value;
			}
		}

		#region Private

		internal class PathFindingNode : IComparable<PathFindingNode> {
			internal PathFindingNode(MapField This) {
				this.This = This;
			}

			public MapField This { get; }
			public PathFindingNode Parent { get; set; } = null;

			public float CostFromStart { get; set; } = 0f;
			public float CostToEnd { get; set; } = float.MinValue;

			public float AllCost => CostFromStart + CostToEnd;

			public static bool operator ==(PathFindingNode one, PathFindingNode two) => one?.This == two?.This;
			public static bool operator !=(PathFindingNode one, PathFindingNode two) => !(one == two);

			public static bool operator <(PathFindingNode one, PathFindingNode two) => one.AllCost < two.AllCost;
			public static bool operator >(PathFindingNode one, PathFindingNode two) => one.AllCost > two.AllCost;


			public bool Equals(PathFindingNode other) {
				return This == other.This;
			}

			public int CompareTo(PathFindingNode two) {
				if ( AllCost < two.AllCost ) return -1;
				if ( AllCost == two.AllCost ) return 0;
				return 1;
			}

			public override bool Equals(object obj) {
				if ( ReferenceEquals(null, obj) ) return false;
				if ( ReferenceEquals(this, obj) ) return true;
				if ( obj.GetType() != GetType() ) return false;

				return Equals((PathFindingNode)obj);
			}

			public override int GetHashCode() {
				return This.GetHashCode();
			}
		}

		private static bool Contains(this IEnumerable<PathFindingNode> list, MapField field) {
			return list.Any(val => val == new PathFindingNode(field));
		}

		#endregion
	}
}