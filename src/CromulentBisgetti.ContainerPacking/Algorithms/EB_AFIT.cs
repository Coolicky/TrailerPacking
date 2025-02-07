using CromulentBisgetti.ContainerPacking.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CromulentBisgetti.ContainerPacking.Algorithms
{
	/// <summary>
	/// A 3D bin packing algorithm originally ported from https://github.com/keremdemirer/3dbinpackingjs,
	/// which itself was a JavaScript port of https://github.com/wknechtel/3d-bin-pack/, which is a C reconstruction 
	/// of a novel algorithm developed in a U.S. Air Force master's thesis by Erhan Baltacioglu in 2001.
	/// </summary>
	public class EB_AFIT : IPackingAlgorithm
	{
		/// <summary>
		/// Runs the packing algorithm.
		/// </summary>
		/// <param name="container">The container to pack items into.</param>
		/// <param name="items">The items to pack.</param>
		/// <returns>The bin packing result.</returns>
		public AlgorithmPackingResult Run(Container container, List<Item> items)
		{
			Initialize(container, items);
			ExecuteIterations(container);
			Report(container);

			var algoResult = new AlgorithmPackingResult();
			algoResult.AlgorithmID = (int)AlgorithmType.EB_AFIT;
			algoResult.AlgorithmName = "EB-AFIT";

			for (var i = 1; i <= itemsToPackCount; i++)
			{
				itemsToPack[i].Quantity = 1;

				if (!itemsToPack[i].IsPacked)
				{
					algoResult.UnpackedItems.Add(itemsToPack[i]);
				}
			}

			algoResult.PackedItems = itemsPackedInOrder;

			if (algoResult.UnpackedItems.Count == 0)
			{
				algoResult.IsCompletePack = true;
			}

			return algoResult;
		}

		private List<BlockedRegion> blockedRegions = new();

		private List<Item> itemsToPack;
		private List<Item> itemsPackedInOrder;
		private List<Layer> layers;
		private ContainerPackingResult result;

		private ScrapPad firstGap;
		private ScrapPad smallestZGap;
		private ScrapPad trashGap;

		private bool isEvened;
		private bool isFullyPacked;
		private bool isLayerComplete;
		private bool isPacking;
		private bool useBestPacking;
		private bool abortPacking;

		private int bestIteration;
		// private int bestBoxIndex;
		private Item bestBox;
		private int currentItemIndex;
		private Item currentBox;
		private int currentBoxIndex;
		private Item candidateBox;
		private int candidateBoxIndex;
		private int layerCount;
		private int packedItemCount;

		private decimal bestBoxFitRemainingX;
		private decimal bestBoxFitRemainingY;
		private decimal bestBoxFitRemainingZ;
		private decimal bestBoxFitX;
		private decimal bestBoxFitY;
		private decimal bestBoxFitZ;
		private decimal bestFitRemainingX;
		private decimal bestFitRemainingY;
		private decimal bestFitRemainingZ;
		private decimal currentBoxWidth;
		private decimal currentBoxHeight;
		private decimal currentBoxDepth;
		private decimal candidateBoxWidth;
		private decimal candidateBoxHeight;
		private decimal candidateBoxDepth;
		private decimal nestedLayerHeight;
		private decimal currentLayerThickness;
		// renamed from lilz;
		private decimal nestedLayerDepth;
		private decimal totalPackedVolume;
		private decimal packedHeight;
		private decimal previousLayerThickness;
		private decimal previousPackedHeight;
		private decimal previousRemainingHeight;
		private decimal containerLength;
		private decimal containerHeight;
		private decimal containerWidth;
		private decimal remainingHeight;
		private decimal remainingDepth;
		private decimal itemsToPackCount;
		private decimal totalItemVolume;
		private decimal totalContainerVolume;

		/// <summary>
		/// Analyzes each unpacked box to find the best fitting one to the empty space given.
		/// </summary>
		private void AnalyzeBox(
			decimal maxWidthAvailable,
			decimal currentHeight,
			decimal maxHeightAvailable,
			decimal currentDepth,
			decimal maxDepthAvailable,
			decimal itemWidth,
			decimal itemHeight,
			decimal itemDepth)
		{
			if (itemWidth <= maxWidthAvailable && itemHeight <= maxHeightAvailable && itemDepth <= maxDepthAvailable)
			{
				// If item height fits in available layer height
				if (itemHeight <= currentHeight)
				{
					if (currentHeight - itemHeight < bestFitRemainingY)
					{
						currentBoxWidth = itemWidth;
						currentBoxHeight = itemHeight;
						currentBoxDepth = itemDepth;
						bestFitRemainingX = maxWidthAvailable - itemWidth;
						bestFitRemainingY = currentHeight - itemHeight;
						bestFitRemainingZ = Math.Abs(currentDepth - itemDepth);
						currentBox = itemsToPack[currentItemIndex];
						currentBoxIndex = currentItemIndex;
					}
					else if (currentHeight - itemHeight == bestFitRemainingY && maxWidthAvailable - itemWidth < bestFitRemainingX)
					{
						currentBoxWidth = itemWidth;
						currentBoxHeight = itemHeight;
						currentBoxDepth = itemDepth;
						bestFitRemainingX = maxWidthAvailable - itemWidth;
						bestFitRemainingY = currentHeight - itemHeight;
						bestFitRemainingZ = Math.Abs(currentDepth - itemDepth);
						currentBox = itemsToPack[currentItemIndex];
						currentBoxIndex = currentItemIndex;
					}
					else if (currentHeight - itemHeight == bestFitRemainingY && maxWidthAvailable - itemWidth == bestFitRemainingX && Math.Abs(currentDepth - itemDepth) < bestFitRemainingZ)
					{
						currentBoxWidth = itemWidth;
						currentBoxHeight = itemHeight;
						currentBoxDepth = itemDepth;
						bestFitRemainingX = maxWidthAvailable - itemWidth;
						bestFitRemainingY = currentHeight - itemHeight;
						bestFitRemainingZ = Math.Abs(currentDepth - itemDepth);
						currentBox = itemsToPack[currentItemIndex];
						currentBoxIndex = currentItemIndex;
					}
				}
				else
				{
					if (itemHeight - currentHeight < bestBoxFitRemainingY)
					{
						bestBoxFitX = itemWidth;
						bestBoxFitY = itemHeight;
						bestBoxFitZ = itemDepth;
						bestBoxFitRemainingX = maxWidthAvailable - itemWidth;
						bestBoxFitRemainingY = itemHeight - currentHeight;
						bestBoxFitRemainingZ = Math.Abs(currentDepth - itemDepth);
						bestBox = itemsToPack[currentItemIndex];
					}
					else if (itemHeight - currentHeight == bestBoxFitRemainingY && maxWidthAvailable - itemWidth < bestBoxFitRemainingX)
					{
						bestBoxFitX = itemWidth;
						bestBoxFitY = itemHeight;
						bestBoxFitZ = itemDepth;
						bestBoxFitRemainingX = maxWidthAvailable - itemWidth;
						bestBoxFitRemainingY = itemHeight - currentHeight;
						bestBoxFitRemainingZ = Math.Abs(currentDepth - itemDepth);
						bestBox = itemsToPack[currentItemIndex];
					}
					else if (itemHeight - currentHeight == bestBoxFitRemainingY && maxWidthAvailable - itemWidth == bestBoxFitRemainingX && Math.Abs(currentDepth - itemDepth) < bestBoxFitRemainingZ)
					{
						bestBoxFitX = itemWidth;
						bestBoxFitY = itemHeight;
						bestBoxFitZ = itemDepth;
						bestBoxFitRemainingX = maxWidthAvailable - itemWidth;
						bestBoxFitRemainingY = itemHeight - currentHeight;
						bestBoxFitRemainingZ = Math.Abs(currentDepth - itemDepth);
						bestBox = itemsToPack[currentItemIndex];
					}
				}
			}
		}

		/// <summary>
		/// After finding each box, the candidate boxes and the condition of the layer are examined.
		/// </summary>
		private void CheckFound()
		{
			isEvened = false;

			if (currentBoxIndex != 0)
			{
				candidateBox = currentBox;
				candidateBoxIndex = currentBoxIndex;
				candidateBoxWidth = currentBoxWidth;
				candidateBoxHeight = currentBoxHeight;
				candidateBoxDepth = currentBoxDepth;
			}
			else
			{
				if (bestBox != null && (nestedLayerHeight != 0 || smallestZGap.Pre == null && smallestZGap.Post == null))
				{
					if (nestedLayerHeight == 0)
					{
						previousLayerThickness = currentLayerThickness;
						nestedLayerDepth = smallestZGap.CumZ;
					}

					candidateBox = bestBox;
					candidateBoxWidth = bestBoxFitX;
					candidateBoxHeight = bestBoxFitY;
					candidateBoxDepth = bestBoxFitZ;
					nestedLayerHeight = nestedLayerHeight + bestBoxFitY - currentLayerThickness;
					currentLayerThickness = bestBoxFitY;
				}
				else
				{
					if (smallestZGap.Pre == null && smallestZGap.Post == null)
					{
						isLayerComplete = true;
					}
					else
					{
						isEvened = true;

						if (smallestZGap.Pre == null)
						{
							trashGap = smallestZGap.Post;
							smallestZGap.CumX = smallestZGap.Post.CumX;
							smallestZGap.CumZ = smallestZGap.Post.CumZ;
							smallestZGap.Post = smallestZGap.Post.Post;
							if (smallestZGap.Post != null)
							{
								smallestZGap.Post.Pre = smallestZGap;
							}
						}
						else if (smallestZGap.Post == null)
						{
							smallestZGap.Pre.Post = null;
							smallestZGap.Pre.CumX = smallestZGap.CumX;
						}
						else
						{
							if (smallestZGap.Pre.CumZ == smallestZGap.Post.CumZ)
							{
								smallestZGap.Pre.Post = smallestZGap.Post.Post;

								if (smallestZGap.Post.Post != null)
								{
									smallestZGap.Post.Post.Pre = smallestZGap.Pre;
								}

								smallestZGap.Pre.CumX = smallestZGap.Post.CumX;
							}
							else
							{
								smallestZGap.Pre.Post = smallestZGap.Post;
								smallestZGap.Post.Pre = smallestZGap.Pre;

								if (smallestZGap.Pre.CumZ < smallestZGap.Post.CumZ)
								{
									smallestZGap.Pre.CumX = smallestZGap.CumX;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Executes the packing algorithm variants.
		/// </summary>
		private void ExecuteIterations(Container container)
		{
			int layersIndex;
			var bestVolume = 0.0M;

			containerLength = container.Length;
			containerHeight = container.Height;
			containerWidth = container.Width;

			layers.Add(new Layer { LayerEval = -1 });
			ListCanditLayers();
			layers = layers.OrderBy(l => l.LayerEval).ToList();

			for (layersIndex = 1; layersIndex <= layerCount && !abortPacking; layersIndex++)
			{
				totalPackedVolume = 0.0M;
				packedHeight = 0;
				isPacking = true;
				currentLayerThickness = layers[layersIndex].LayerDim;
				var itelayer = layersIndex;
				remainingHeight = containerHeight;
				remainingDepth = containerWidth;
				packedItemCount = 0;

				for (currentItemIndex = 1; currentItemIndex <= itemsToPackCount; currentItemIndex++)
				{
					itemsToPack[currentItemIndex].IsPacked = false;
				}

				do
				{
					nestedLayerHeight = 0;
					isLayerComplete = false;

					PackLayer();

					packedHeight += currentLayerThickness;
					remainingHeight = containerHeight - packedHeight;

					if (nestedLayerHeight != 0 && !abortPacking)
					{
						previousPackedHeight = packedHeight;
						previousRemainingHeight = remainingHeight;
						remainingHeight = currentLayerThickness - previousLayerThickness;
						packedHeight = packedHeight - currentLayerThickness + previousLayerThickness;
						remainingDepth = nestedLayerDepth;
						currentLayerThickness = nestedLayerHeight;
						isLayerComplete = false;

						PackLayer();

						packedHeight = previousPackedHeight;
						remainingHeight = previousRemainingHeight;
						remainingDepth = containerWidth;
					}

					FindLayer(remainingHeight);
				} while (isPacking && !abortPacking);

				var packeditems = itemsToPack.Where(i => i.IsPacked).ToList();
				Log.Information("TotalPackedVolume! -> " + totalPackedVolume);
				if (totalPackedVolume > bestVolume && !abortPacking)
				{
					bestVolume = totalPackedVolume;
					bestIteration = itelayer;
				}

				if (isFullyPacked) break;
				blockedRegions.Clear();
			}

			layers = new List<Layer>();
		}

		/// <summary>
		/// Finds the most proper boxes by looking at all six possible orientations,
		/// empty space given, adjacent boxes, and pallet limits.
		/// </summary>
		private void FindBox(decimal hmx, decimal hy, decimal hmy, decimal hz, decimal hmz)
		{
			int y;
			bestFitRemainingX = 32767;
			bestFitRemainingY = 32767;
			bestFitRemainingZ = 32767;
			bestBoxFitRemainingX = 32767;
			bestBoxFitRemainingY = 32767;
			bestBoxFitRemainingZ = 32767;
			currentBox = null;
			currentBoxIndex = 0;
			bestBox = null;

			for (y = 1; y <= itemsToPackCount; y += itemsToPack[y].Quantity)
			{
				for (currentItemIndex = y; currentItemIndex < currentItemIndex + itemsToPack[y].Quantity - 1; currentItemIndex++)
				{
					if (!itemsToPack[currentItemIndex].IsPacked) break;
				}

				if (itemsToPack[currentItemIndex].IsPacked) continue;

				if (currentItemIndex > itemsToPackCount) return;

				if (IsOverlapping())
				{
					continue;
				} 

				AnalyzeBox(hmx, hy, hmy, hz, hmz, itemsToPack[currentItemIndex].Dim1, itemsToPack[currentItemIndex].Dim2, itemsToPack[currentItemIndex].Dim3);

				if (itemsToPack[currentItemIndex].Dim1 == itemsToPack[currentItemIndex].Dim3 &&
				    itemsToPack[currentItemIndex].Dim3 == itemsToPack[currentItemIndex].Dim2)
					continue;

				//TODO: Not rotated!!!
				AnalyzeBox(hmx, hy, hmy, hz, hmz, itemsToPack[currentItemIndex].Dim1, itemsToPack[currentItemIndex].Dim3, itemsToPack[currentItemIndex].Dim2);
				AnalyzeBox(hmx, hy, hmy, hz, hmz, itemsToPack[currentItemIndex].Dim2, itemsToPack[currentItemIndex].Dim1, itemsToPack[currentItemIndex].Dim3);
				AnalyzeBox(hmx, hy, hmy, hz, hmz, itemsToPack[currentItemIndex].Dim2, itemsToPack[currentItemIndex].Dim3, itemsToPack[currentItemIndex].Dim1);
				AnalyzeBox(hmx, hy, hmy, hz, hmz, itemsToPack[currentItemIndex].Dim3, itemsToPack[currentItemIndex].Dim1, itemsToPack[currentItemIndex].Dim2);
				AnalyzeBox(hmx, hy, hmy, hz, hmz, itemsToPack[currentItemIndex].Dim3, itemsToPack[currentItemIndex].Dim2, itemsToPack[currentItemIndex].Dim1);
			}
			Log.Information($"BestBox-> {bestBox?.Dim1}|{bestBox?.Dim2}|{bestBox?.Dim3}");
			Log.Information($"Layer-> {currentLayerThickness}");
		}
		private bool IsOverlapping()
		{

			var xStart = itemsToPack[currentItemIndex].CoordX;
			var zStart = itemsToPack[currentItemIndex].CoordZ;
			var xEnd = xStart + itemsToPack[currentItemIndex].Dim1;
			var zEnd = zStart + itemsToPack[currentItemIndex].Dim3;

			var isOverlapping = blockedRegions.Any(r =>
				// packedHeight >= r.MinYLevel &&
				xStart < r.XEnd &&
				xEnd > r.XStart &&
				zStart < r.ZEnd &&
				zEnd > r.ZStart);
			return isOverlapping;
		}

		/// <summary>
		/// Finds the most proper layer height by looking at the unpacked boxes and the remaining empty space available.
		/// </summary>
		private void FindLayer(decimal thickness)
		{
			decimal exdim = 0;
			decimal dimdif;
			decimal dimen2 = 0;
			decimal dimen3 = 0;
			int y;
			int z;
			decimal layereval;
			decimal eval;
			currentLayerThickness = 0;
			eval = 1000000;

			for (currentItemIndex = 1; currentItemIndex <= itemsToPackCount; currentItemIndex++)
			{
				if (itemsToPack[currentItemIndex].IsPacked) continue;

				for (y = 1; y <= 3; y++)
				{
					switch (y)
					{
						case 1:
							exdim = itemsToPack[currentItemIndex].Dim1;
							dimen2 = itemsToPack[currentItemIndex].Dim2;
							dimen3 = itemsToPack[currentItemIndex].Dim3;
							break;

						case 2:
							exdim = itemsToPack[currentItemIndex].Dim2;
							dimen2 = itemsToPack[currentItemIndex].Dim1;
							dimen3 = itemsToPack[currentItemIndex].Dim3;
							break;

						case 3:
							exdim = itemsToPack[currentItemIndex].Dim3;
							dimen2 = itemsToPack[currentItemIndex].Dim1;
							dimen3 = itemsToPack[currentItemIndex].Dim2;
							break;
					}

					layereval = 0;

					if (exdim <= thickness && (dimen2 <= containerLength && dimen3 <= containerWidth || dimen3 <= containerLength && dimen2 <= containerWidth))
					{
						for (z = 1; z <= itemsToPackCount; z++)
						{
							if (!(currentItemIndex == z) && !itemsToPack[z].IsPacked)
							{
								dimdif = Math.Abs(exdim - itemsToPack[z].Dim1);

								if (Math.Abs(exdim - itemsToPack[z].Dim2) < dimdif)
								{
									dimdif = Math.Abs(exdim - itemsToPack[z].Dim2);
								}

								if (Math.Abs(exdim - itemsToPack[z].Dim3) < dimdif)
								{
									dimdif = Math.Abs(exdim - itemsToPack[z].Dim3);
								}

								layereval += dimdif;
							}
						}

						if (layereval < eval)
						{
							eval = layereval;
							currentLayerThickness = exdim;
						}
					}
				}
			}

			if (currentLayerThickness == 0 || currentLayerThickness > remainingHeight) isPacking = false;
		}

		/// <summary>
		/// Finds the first to be packed gap in the layer edge.
		/// </summary>
		private void FindSmallestZ()
		{
			var scrapmemb = firstGap;
			smallestZGap = scrapmemb;

			while (scrapmemb.Post != null)
			{
				if (scrapmemb.Post.CumZ < smallestZGap.CumZ)
				{
					smallestZGap = scrapmemb.Post;
				}

				scrapmemb = scrapmemb.Post;
			}
		}

		/// <summary>
		/// Initializes everything.
		/// </summary>
		private void Initialize(Container container, List<Item> items)
		{
			itemsToPack = new List<Item>();
			itemsPackedInOrder = new List<Item>();
			result = new ContainerPackingResult();

			// The original code uses 1-based indexing everywhere. This fake entry is added to the beginning
			// of the list to make that possible.
			itemsToPack.Add(new Item(0, 0, 0, 0, 0, true));

			layers = new List<Layer>();
			itemsToPackCount = 0;

			foreach (var item in items)
			{
				for (var i = 1; i <= item.Quantity; i++)
				{
					var newItem = new Item(item.ID, item.Dim1, item.Dim2, item.Dim3, item.Quantity, item.IsStackable);
					itemsToPack.Add(newItem);
				}

				itemsToPackCount += item.Quantity;
			}

			itemsToPack.Add(new Item(0, 0, 0, 0, 0, true));

			totalContainerVolume = container.Length * container.Height * container.Width;
			totalItemVolume = 0.0M;

			for (currentItemIndex = 1; currentItemIndex <= itemsToPackCount; currentItemIndex++)
			{
				totalItemVolume += itemsToPack[currentItemIndex].Volume;
			}

			firstGap = new ScrapPad();

			firstGap.Pre = null;
			firstGap.Post = null;
			useBestPacking = false;
			isFullyPacked = false;
			abortPacking = false;
		}

		/// <summary>
		/// Lists all possible layer heights by giving a weight value to each of them.
		/// </summary>
		private void ListCanditLayers()
		{
			bool same;
			decimal exdim = 0;
			decimal dimdif;
			decimal dimen2 = 0;
			decimal dimen3 = 0;
			int y;
			int z;
			int k;
			decimal layereval;

			layerCount = 0;

			for (currentItemIndex = 1; currentItemIndex <= itemsToPackCount; currentItemIndex++)
			{
				for (y = 1; y <= 3; y++)
				{
					switch (y)
					{
						case 1:
							exdim = itemsToPack[currentItemIndex].Dim1;
							dimen2 = itemsToPack[currentItemIndex].Dim2;
							dimen3 = itemsToPack[currentItemIndex].Dim3;
							break;

						case 2:
							exdim = itemsToPack[currentItemIndex].Dim2;
							dimen2 = itemsToPack[currentItemIndex].Dim1;
							dimen3 = itemsToPack[currentItemIndex].Dim3;
							break;

						case 3:
							exdim = itemsToPack[currentItemIndex].Dim3;
							dimen2 = itemsToPack[currentItemIndex].Dim1;
							dimen3 = itemsToPack[currentItemIndex].Dim2;
							break;
					}

					if (exdim > containerHeight || (dimen2 > containerLength || dimen3 > containerWidth) && (dimen3 > containerLength || dimen2 > containerWidth)) continue;

					same = false;

					for (k = 1; k <= layerCount; k++)
					{
						if (exdim == layers[k].LayerDim)
						{
							same = true;
							continue;
						}
					}

					if (same) continue;

					layereval = 0;

					for (z = 1; z <= itemsToPackCount; z++)
					{
						if (!(currentItemIndex == z))
						{
							dimdif = Math.Abs(exdim - itemsToPack[z].Dim1);

							if (Math.Abs(exdim - itemsToPack[z].Dim2) < dimdif)
							{
								dimdif = Math.Abs(exdim - itemsToPack[z].Dim2);
							}
							if (Math.Abs(exdim - itemsToPack[z].Dim3) < dimdif)
							{
								dimdif = Math.Abs(exdim - itemsToPack[z].Dim3);
							}
							layereval += dimdif;
						}
					}

					layerCount++;

					layers.Add(new Layer());
					layers[layerCount].LayerEval = layereval;
					layers[layerCount].LayerDim = exdim;
				}
			}
		}

		/// <summary>
		/// Packs the boxes found and arranges all variables and records properly.
		/// </summary>
		private void PackLayer()
		{
			decimal lenx;
			decimal lenz;
			decimal lpz;

			if (currentLayerThickness == 0)
			{
				isPacking = false;
				return;
			}

			firstGap.CumX = containerLength;
			firstGap.CumZ = 0;

			for (; !abortPacking;)
			{
				FindSmallestZ();

				if (smallestZGap.Pre == null && smallestZGap.Post == null)
				{
					//*** SITUATION-1: NO BOXES ON THE RIGHT AND LEFT SIDES ***

					lenx = smallestZGap.CumX;
					lpz = remainingDepth - smallestZGap.CumZ;
					FindBox(lenx, currentLayerThickness, remainingHeight, lpz, lpz);
					CheckFound();

					if (isLayerComplete) break;
					if (isEvened) continue;

					itemsToPack[candidateBoxIndex].CoordX = 0;
					itemsToPack[candidateBoxIndex].CoordY = packedHeight;
					itemsToPack[candidateBoxIndex].CoordZ = smallestZGap.CumZ;
					if (candidateBoxWidth == smallestZGap.CumX)
					{
						smallestZGap.CumZ += candidateBoxDepth;
					}
					else
					{
						smallestZGap.Post = new ScrapPad();

						smallestZGap.Post.Post = null;
						smallestZGap.Post.Pre = smallestZGap;
						smallestZGap.Post.CumX = smallestZGap.CumX;
						smallestZGap.Post.CumZ = smallestZGap.CumZ;
						smallestZGap.CumX = candidateBoxWidth;
						smallestZGap.CumZ += candidateBoxDepth;
					}
				}
				else if (smallestZGap.Pre == null)
				{
					//*** SITUATION-2: NO BOXES ON THE LEFT SIDE ***

					lenx = smallestZGap.CumX;
					lenz = smallestZGap.Post.CumZ - smallestZGap.CumZ;
					lpz = remainingDepth - smallestZGap.CumZ;
					FindBox(lenx, currentLayerThickness, remainingHeight, lenz, lpz);
					CheckFound();

					if (isLayerComplete) break;
					if (isEvened) continue;

					itemsToPack[candidateBoxIndex].CoordY = packedHeight;
					itemsToPack[candidateBoxIndex].CoordZ = smallestZGap.CumZ;
					if (candidateBoxWidth == smallestZGap.CumX)
					{
						itemsToPack[candidateBoxIndex].CoordX = 0;

						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Post.CumZ)
						{
							smallestZGap.CumZ = smallestZGap.Post.CumZ;
							smallestZGap.CumX = smallestZGap.Post.CumX;
							trashGap = smallestZGap.Post;
							smallestZGap.Post = smallestZGap.Post.Post;

							if (smallestZGap.Post != null)
							{
								smallestZGap.Post.Pre = smallestZGap;
							}
						}
						else
						{
							smallestZGap.CumZ += candidateBoxDepth;
						}
					}
					else
					{
						itemsToPack[candidateBoxIndex].CoordX = smallestZGap.CumX - candidateBoxWidth;

						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Post.CumZ)
						{
							smallestZGap.CumX -= candidateBoxWidth;
						}
						else
						{
							smallestZGap.Post.Pre = new ScrapPad();

							smallestZGap.Post.Pre.Post = smallestZGap.Post;
							smallestZGap.Post.Pre.Pre = smallestZGap;
							smallestZGap.Post = smallestZGap.Post.Pre;
							smallestZGap.Post.CumX = smallestZGap.CumX;
							smallestZGap.CumX -= candidateBoxWidth;
							smallestZGap.Post.CumZ = smallestZGap.CumZ + candidateBoxDepth;
						}
					}
				}
				else if (smallestZGap.Post == null)
				{
					//*** SITUATION-3: NO BOXES ON THE RIGHT SIDE ***

					lenx = smallestZGap.CumX - smallestZGap.Pre.CumX;
					lenz = smallestZGap.Pre.CumZ - smallestZGap.CumZ;
					lpz = remainingDepth - smallestZGap.CumZ;
					FindBox(lenx, currentLayerThickness, remainingHeight, lenz, lpz);
					CheckFound();

					if (isLayerComplete) break;
					if (isEvened) continue;

					itemsToPack[candidateBoxIndex].CoordY = packedHeight;
					itemsToPack[candidateBoxIndex].CoordZ = smallestZGap.CumZ;
					itemsToPack[candidateBoxIndex].CoordX = smallestZGap.Pre.CumX;

					if (candidateBoxWidth == smallestZGap.CumX - smallestZGap.Pre.CumX)
					{
						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Pre.CumZ)
						{
							smallestZGap.Pre.CumX = smallestZGap.CumX;
							smallestZGap.Pre.Post = null;
						}
						else
						{
							smallestZGap.CumZ += candidateBoxDepth;
						}
					}
					else
					{
						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Pre.CumZ)
						{
							smallestZGap.Pre.CumX += candidateBoxWidth;
						}
						else
						{
							smallestZGap.Pre.Post = new ScrapPad();

							smallestZGap.Pre.Post.Pre = smallestZGap.Pre;
							smallestZGap.Pre.Post.Post = smallestZGap;
							smallestZGap.Pre = smallestZGap.Pre.Post;
							smallestZGap.Pre.CumX = smallestZGap.Pre.Pre.CumX + candidateBoxWidth;
							smallestZGap.Pre.CumZ = smallestZGap.CumZ + candidateBoxDepth;
						}
					}
				}
				else if (smallestZGap.Pre.CumZ == smallestZGap.Post.CumZ)
				{
					//*** SITUATION-4: THERE ARE BOXES ON BOTH OF THE SIDES ***

					//*** SUBSITUATION-4A: SIDES ARE EQUAL TO EACH OTHER ***

					lenx = smallestZGap.CumX - smallestZGap.Pre.CumX;
					lenz = smallestZGap.Pre.CumZ - smallestZGap.CumZ;
					lpz = remainingDepth - smallestZGap.CumZ;

					FindBox(lenx, currentLayerThickness, remainingHeight, lenz, lpz);
					CheckFound();

					if (isLayerComplete) break;
					if (isEvened) continue;

					itemsToPack[candidateBoxIndex].CoordY = packedHeight;
					itemsToPack[candidateBoxIndex].CoordZ = smallestZGap.CumZ;

					if (candidateBoxWidth == smallestZGap.CumX - smallestZGap.Pre.CumX)
					{
						itemsToPack[candidateBoxIndex].CoordX = smallestZGap.Pre.CumX;

						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Post.CumZ)
						{
							smallestZGap.Pre.CumX = smallestZGap.Post.CumX;

							if (smallestZGap.Post.Post != null)
							{
								smallestZGap.Pre.Post = smallestZGap.Post.Post;
								smallestZGap.Post.Post.Pre = smallestZGap.Pre;
							}
							else
							{
								smallestZGap.Pre.Post = null;
							}
						}
						else
						{
							smallestZGap.CumZ += candidateBoxDepth;
						}
					}
					else if (smallestZGap.Pre.CumX < containerLength - smallestZGap.CumX)
					{
						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Pre.CumZ)
						{
							smallestZGap.CumX -= candidateBoxWidth;
							itemsToPack[candidateBoxIndex].CoordX = smallestZGap.CumX;
						}
						else
						{
							itemsToPack[candidateBoxIndex].CoordX = smallestZGap.Pre.CumX;
							smallestZGap.Pre.Post = new ScrapPad();

							smallestZGap.Pre.Post.Pre = smallestZGap.Pre;
							smallestZGap.Pre.Post.Post = smallestZGap;
							smallestZGap.Pre = smallestZGap.Pre.Post;
							smallestZGap.Pre.CumX = smallestZGap.Pre.Pre.CumX + candidateBoxWidth;
							smallestZGap.Pre.CumZ = smallestZGap.CumZ + candidateBoxDepth;
						}
					}
					else
					{
						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Pre.CumZ)
						{
							smallestZGap.Pre.CumX += candidateBoxWidth;
							itemsToPack[candidateBoxIndex].CoordX = smallestZGap.Pre.CumX;
						}
						else
						{
							itemsToPack[candidateBoxIndex].CoordX = smallestZGap.CumX - candidateBoxWidth;
							smallestZGap.Post.Pre = new ScrapPad();

							smallestZGap.Post.Pre.Post = smallestZGap.Post;
							smallestZGap.Post.Pre.Pre = smallestZGap;
							smallestZGap.Post = smallestZGap.Post.Pre;
							smallestZGap.Post.CumX = smallestZGap.CumX;
							smallestZGap.Post.CumZ = smallestZGap.CumZ + candidateBoxDepth;
							smallestZGap.CumX -= candidateBoxWidth;
						}
					}
				}
				else
				{
					//*** SUBSITUATION-4B: SIDES ARE NOT EQUAL TO EACH OTHER ***

					lenx = smallestZGap.CumX - smallestZGap.Pre.CumX;
					lenz = smallestZGap.Pre.CumZ - smallestZGap.CumZ;
					lpz = remainingDepth - smallestZGap.CumZ;
					FindBox(lenx, currentLayerThickness, remainingHeight, lenz, lpz);
					CheckFound();

					if (isLayerComplete) break;
					if (isEvened) continue;

					itemsToPack[candidateBoxIndex].CoordY = packedHeight;
					itemsToPack[candidateBoxIndex].CoordZ = smallestZGap.CumZ;
					itemsToPack[candidateBoxIndex].CoordX = smallestZGap.Pre.CumX;

					if (candidateBoxWidth == smallestZGap.CumX - smallestZGap.Pre.CumX)
					{
						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Pre.CumZ)
						{
							smallestZGap.Pre.CumX = smallestZGap.CumX;
							smallestZGap.Pre.Post = smallestZGap.Post;
							smallestZGap.Post.Pre = smallestZGap.Pre;
						}
						else
						{
							smallestZGap.CumZ += candidateBoxDepth;
						}
					}
					else
					{
						if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Pre.CumZ)
						{
							smallestZGap.Pre.CumX += candidateBoxWidth;
						}
						else if (smallestZGap.CumZ + candidateBoxDepth == smallestZGap.Post.CumZ)
						{
							itemsToPack[candidateBoxIndex].CoordX = smallestZGap.CumX - candidateBoxWidth;
							smallestZGap.CumX -= candidateBoxWidth;
						}
						else
						{
							smallestZGap.Pre.Post = new ScrapPad();

							smallestZGap.Pre.Post.Pre = smallestZGap.Pre;
							smallestZGap.Pre.Post.Post = smallestZGap;
							smallestZGap.Pre = smallestZGap.Pre.Post;
							smallestZGap.Pre.CumX = smallestZGap.Pre.Pre.CumX + candidateBoxWidth;
							smallestZGap.Pre.CumZ = smallestZGap.CumZ + candidateBoxDepth;
						}
					}
				}

				VolumeCheck();
			}
		}

		/// <summary>
		/// Using the parameters found, packs the best solution found and
		/// reports to the console.
		/// </summary>
		private void Report(Container container)
		{
			blockedRegions.Clear();
			abortPacking = false;

			containerLength = container.Length;
			containerHeight = container.Height;
			containerWidth = container.Width;

			useBestPacking = true;

			layers.Clear();
			layers.Add(new Layer { LayerEval = -1 });
			ListCanditLayers();
			layers = layers.OrderBy(l => l.LayerEval).ToList();
			totalPackedVolume = 0;
			packedHeight = 0;
			isPacking = true;
			currentLayerThickness = layers[bestIteration].LayerDim;
			remainingHeight = containerHeight;
			remainingDepth = containerWidth;

			for (currentItemIndex = 1; currentItemIndex <= itemsToPackCount; currentItemIndex++)
			{
				itemsToPack[currentItemIndex].IsPacked = false;
			}

			do
			{
				nestedLayerHeight = 0;
				isLayerComplete = false;
				PackLayer();
				packedHeight += currentLayerThickness;
				remainingHeight = containerHeight - packedHeight;

				if (nestedLayerHeight > 0.0001M)
				{
					previousPackedHeight = packedHeight;
					previousRemainingHeight = remainingHeight;
					remainingHeight = currentLayerThickness - previousLayerThickness;
					packedHeight = packedHeight - currentLayerThickness + previousLayerThickness;
					remainingDepth = nestedLayerDepth;
					currentLayerThickness = nestedLayerHeight;
					isLayerComplete = false;
					PackLayer();
					packedHeight = previousPackedHeight;
					remainingHeight = previousRemainingHeight;
					remainingDepth = containerWidth;
				}

				if (!abortPacking)
				{
					FindLayer(remainingHeight);
				}
			} while (isPacking && !abortPacking);
		}

		/// <summary>
		/// After packing of each item, the 100% packing condition is checked.
		/// </summary>
		private void VolumeCheck()
		{
			itemsToPack[candidateBoxIndex].IsPacked = true;
			itemsToPack[candidateBoxIndex].PackDimX = candidateBoxWidth;
			itemsToPack[candidateBoxIndex].PackDimY = candidateBoxHeight;
			itemsToPack[candidateBoxIndex].PackDimZ = candidateBoxDepth;
			totalPackedVolume += itemsToPack[candidateBoxIndex].Volume;
			packedItemCount++;

			if (!itemsToPack[candidateBoxIndex].IsStackable)
			{
				var x1 = itemsToPack[candidateBoxIndex].CoordX;
				var z1 = itemsToPack[candidateBoxIndex].CoordZ;
				var x2 = x1 + itemsToPack[candidateBoxIndex].PackDimX;
				var z2 = z1 + itemsToPack[candidateBoxIndex].PackDimZ;
				var minY = packedHeight + candidateBoxHeight;
				blockedRegions.Add(new BlockedRegion(x1, x2, z1, z2, minY));
			}

			if (useBestPacking)
			{
				Log.Information($"Packing->{itemsToPack[candidateBoxIndex].PackDimX}|{itemsToPack[candidateBoxIndex].PackDimY}|{itemsToPack[candidateBoxIndex].PackDimZ}");
				itemsPackedInOrder.Add(itemsToPack[candidateBoxIndex]);
			}
			else if (totalPackedVolume == totalContainerVolume || totalPackedVolume == totalItemVolume)
			{
				isPacking = false;
				isFullyPacked = true;
			}
		}

		/// <summary>
		/// A list that stores all the different lengths of all item dimensions.
		/// From the master's thesis:
		/// "Each Layerdim value in this array represents a different layer thickness
		/// value with which each iteration can start packing. Before starting iterations,
		/// all different lengths of all box dimensions along with evaluation values are
		/// stored in this array" (p. 3-6).
		/// </summary>
		private class Layer
		{
			/// <summary>
			/// Gets or sets the layer dimension value, representing a layer thickness.
			/// </summary>
			/// <value>
			/// The layer dimension value.
			/// </value>
			public decimal LayerDim { get; set; }

			/// <summary>
			/// Gets or sets the layer eval value, representing an evaluation weight
			/// value for the corresponding LayerDim value.
			/// </summary>
			/// <value>
			/// The layer eval value.
			/// </value>
			public decimal LayerEval { get; set; }
		}

		/// <summary>
		/// From the master's thesis:
		/// "The double linked list we use keeps the topology of the edge of the 
		/// current layer under construction. We keep the x and z coordinates of 
		/// each gap's right corner. The program looks at those gaps and tries to 
		/// fill them with boxes one at a time while trying to keep the edge of the
		/// layer even" (p. 3-7).
		/// </summary>
		private class ScrapPad
		{
			/// <summary>
			/// Gets or sets the x coordinate of the gap's right corner.
			/// </summary>
			/// <value>
			/// The x coordinate of the gap's right corner.
			/// </value>
			public decimal CumX { get; set; }

			/// <summary>
			/// Gets or sets the z coordinate of the gap's right corner.
			/// </summary>
			/// <value>
			/// The z coordinate of the gap's right corner.
			/// </value>
			public decimal CumZ { get; set; }

			/// <summary>
			/// Gets or sets the following entry.
			/// </summary>
			/// <value>
			/// The following entry.
			/// </value>
			public ScrapPad Post { get; set; }

			/// <summary>
			/// Gets or sets the previous entry.
			/// </summary>
			/// <value>
			/// The previous entry.
			/// </value>
			public ScrapPad Pre { get; set; }
		}

		private record BlockedRegion
		{
			public BlockedRegion(decimal xStart, decimal xEnd, decimal zStart, decimal zEnd, decimal minY)
			{
				XStart = xStart;
				XEnd = xEnd;
				ZStart = zStart;
				ZEnd = zEnd;
				MinYLevel = minY;
			}

			public decimal XStart { get; set; }
			public decimal XEnd { get; set; }
			public decimal ZStart { get; set; }
			public decimal ZEnd { get; set; }
			public decimal MinYLevel { get; set; }
		}
	}
}