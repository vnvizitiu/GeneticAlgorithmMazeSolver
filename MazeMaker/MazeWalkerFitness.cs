﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
namespace MazeMaker
{
	using System.Runtime.CompilerServices;

	public class MazeWalkerFitness : IFitness
	{
		private readonly Maze _maze;
		public MazeWalkerFitness(Maze maze)
		{
			_maze = maze;
		}
		public double Evaluate(IChromosome chromosome)
		{
			var m = _maze.Copy();
			var walker = new MazeWalker(m,string.Join("",chromosome.GetGenes().Select(g=>g.Value as string)));
			int repeatedSteps;
			var steps = walker.Walk(out repeatedSteps);

			if (steps < int.MaxValue)
			{
				// get rid of steps that occur after a solution is found
				// chromosome.Resize(steps);

				steps = steps + repeatedSteps;

			}
			else
			{

				steps -= (this._maze.Height * this._maze.Width)*50; //prevent overflow
				var open = 0;
				double closest = int.MaxValue;

				for (int x = 0; x < this._maze.Width; x++)
				{
					for (int y = 0; y < this._maze.Height; y++)
					{
						if (m[x, y] == Maze.State.Open)
						{
							open++;

						}	
						else if (m[x, y] == Maze.State.Walked || m[x, y] == Maze.State.MultiWalked)
						{
							var distance = Math.Sqrt(Math.Pow(m.End.X - x, 2) + Math.Pow(m.End.Y - y, 2));
							if (distance < closest)
							{
								closest = distance;
							}
						}


					}

				}
				steps += (open*50) + ((int)closest*2);


			}
			return -(steps);
		}
	}
}