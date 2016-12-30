using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using System.Threading.Tasks;
using PracaInzynierska.Utils.Diagnostic;
using SFML.Graphics;
using static PracaInzynierska.Utils.Algorithm.PathFinding.Metric;
using static PracaInzynierska.Utils.Algorithm.PathFinding;
using static PracaInzynierska.Textures.MapTextures;

namespace PracaInzynierska.Utils.Diagnostic {

	public static class PathFinding {
		/// <summary>
		/// Algorytm A* znajdowania najkrotszej sciezki na planszy.
		/// </summary>
		/// <param name="from">Pole startowe</param>
		/// <param name="to">Pole, ktore chcemy osiagnac</param>
		/// <param name="heuristic">Metryka w jakiej maja byc szacowana odleglosc do pola docelowego</param>
		/// <param name="forbidenFields">Pola przez które nie wolno przejść przy danym poszukiwaniu ścieżki</param>
		/// <returns>Lista pol skladajaca sie na sciezke</returns>
		public static IList<MapField> AStar(MapField from, MapField to, heuristicFunc heuristic, ref AStarDiagnostic diagnostic, params MapField[] forbidenFields) {
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
		        ++diagnostic.Iterations;

				PathFindingNode current = openList.RemoveAtAndGet(0);
				closeList.Add(current);

		        if ( heuristic == NullDistance && current.This == to ) {
					return ReconstructPath(current, ref diagnostic);
				} else if ( current.This == to ) { // znaleziono pole do ktorego dazylismy
			        reachedGoal = true;
		        } else if ( reachedGoal ) {
			        PathFindingNode endNode = openList.Concat(closeList)
													  .First(node => node.This == to);

			        float minValInOpenList = openList.Select(node => node.CostFromStart)
													 .Min();

			        if ( minValInOpenList < endNode.CostFromStart ) return ReconstructPath(endNode, ref diagnostic);
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
		private static IList<MapField> ReconstructPath(PathFindingNode node, ref AStarDiagnostic diagnostic) {
			List<MapField> path = new List<MapField>();
			diagnostic.TotalCost = node.CostFromStart;

		    while ( true ) {
			    ++diagnostic.TotalLength;
		        path.Add(node.This);
			    if ( node.Parent == null ) { break; }
				node = node.Parent;
		    }

		    path.Reverse();
			return path;
		}

		public static IList<MapField> AStar(MapField from, MapField to, heuristicFunc heuristic,
											ref AStarDiagnostic diagnostic, Map.Map map, params MapField[] forbidenFields) {
			if ( (from == null) || (to == null) ) throw new NullReferenceException(); // ktores z pol nie istnieje
			if ( !to.IsAvaliable ) throw new FieldNotAvaliableException(); // pole jest niedostepne

			if ( from.Neighbour.Contains(to) )
				return new List<MapField>(2) {
												 from,
												 to,
											 }; // pola from i to leza kolo siebie

			RenderTexture render = new RenderTexture((uint)(25 * MapField.ScreenSize),
													 (uint)(25 * MapField.ScreenSize), true) {
																								 Smooth = true,
																							 };
			render.Draw(map);
			Image image = render.Texture.CopyToImage();
			image.SaveToFile($@"map_2_E_0.png");

			//Lita pol do przeszukania
			List<PathFindingNode> openList = new List<PathFindingNode> {
																		   new PathFindingNode(from)
																	   };
			//Lista przeszukanych pol
			List<PathFindingNode> closeList =
					new List<PathFindingNode>(forbidenFields.Select(forbidenField => new PathFindingNode(forbidenField)));

			bool achivewedGoal = false;

			//dopoki jakiekolwiek pole moze zostac jeszcze przebadane
			while ( openList.Count != 0 ) {
				++diagnostic.Iterations;

				PathFindingNode current = openList.RemoveAtAndGet(0);

				render.Clear();
				render.Draw(map);

				render.Draw(new Sprite(SelectedTexture) {
															Position = from.ScreenPosition
														});
				render.Draw(new Sprite(SelectedTexture) {
															Position = to.ScreenPosition
														});
				foreach ( Sprite val in openList.Select(field => new Sprite(OpenListTexture) {
																								 Position = field.This.ScreenPosition
																							 }) ) {
					render.Draw(val);
				}
				foreach ( Sprite val in closeList.Select(field => new Sprite(CloseListTexture) {
																								   Position = field.This.ScreenPosition
																							   }) ) {
					render.Draw(val);
				}

				render.Draw(new Sprite(CurrentFieldTexture) {
																Position = current.This.ScreenPosition
															});
				render.Texture.CopyToImage()
					  .SaveToFile($@"map_2_E_{diagnostic.Iterations}.png");

				closeList.Add(current);

				if ( heuristic == NullDistance && current.This == to ) {
					return ReconstructPath(current, ref diagnostic);
				} else if ( current.This == to ) { // znaleziono pole do ktorego dazylismy
					achivewedGoal = true;
				} else if ( achivewedGoal ) {
					PathFindingNode endNode = openList.Concat(closeList)
													  .First(node => node.This == to);

					float minValInOpenList = openList.Select(node => node.CostFromStart)
													 .Min();

					if ( minValInOpenList < endNode.CostFromStart ) return ReconstructPath(endNode, ref diagnostic, render);
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
					} else {
						openList.Add(neighbourNode);
					}
				}

				//openList.AsParallel().WithDegreeOfParallelism(4).OrderBy(field => field.AllCost);
				openList.Sort();
			}

			//jesli niedotarto do zadanego celu
			throw new FieldNotAvaliableException();
		}

		private static IList<MapField> ReconstructPath(PathFindingNode node, ref AStarDiagnostic diagnostic, RenderTexture render)
		{
			List<MapField> path = new List<MapField>();
			diagnostic.TotalCost = node.CostFromStart;

			while (true)
			{
				++diagnostic.TotalLength;
				path.Add(node.This);
				render.Draw(new Sprite(SelectedTexture) {
															Position = node.This.ScreenPosition
				} );
				if (node.Parent == null) { break; }
				node = node.Parent;
			}

			path.Reverse();
			render.Texture.CopyToImage()
				  .SaveToFile($"End.png");
			return path;
		}
	}
}