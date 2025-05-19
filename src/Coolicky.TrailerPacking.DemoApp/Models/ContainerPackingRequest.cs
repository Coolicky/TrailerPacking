﻿using Coolicky.ContainerPacking.Entities;
using System.Collections.Generic;

namespace Coolicky.TrailerPacking.DemoApp.Models
{
	public class ContainerPackingRequest
	{
		public List<Container> Containers { get; set; }

		public List<RequestItem> ItemsToPack { get; set; }

		public List<int> AlgorithmTypeIDs { get; set; }
	}
}