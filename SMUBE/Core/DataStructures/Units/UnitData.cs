﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Units
{
    public class UnitData
    {
        public const int DEFAULT_UNIT_GRID_SIZE = 1;
        public string Name { get; private set; }
        public UnitIdentifier UnitIdentifier { get; }
        public UnitStats UnitStats { get; }

        public UnitData(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats)
        {
            Name = argName;
            UnitIdentifier = argUnitIdentifier;
            UnitStats = argUnitStats;
        }

        private UnitData(UnitData sourceUnitData)
        {
            UnitIdentifier = sourceUnitData.UnitIdentifier;
            Name = sourceUnitData.Name;
            UnitStats = sourceUnitData.UnitStats.Copy();
        }

        public UnitData DeepCopy()
        {
            return new UnitData(this);
        }

        public override string ToString()
        {
            var fullDescribedStatus = new StringBuilder();

            fullDescribedStatus.AppendLine($"Unit name: {Name}");
            fullDescribedStatus.AppendLine($"Unit team: {UnitIdentifier.TeamId}");
            fullDescribedStatus.Append(UnitStats.ToString());

            return fullDescribedStatus.ToString();
        }

        public string ToShortString()
        {
            var fullDescribedStatus = new StringBuilder();

            fullDescribedStatus.AppendLine($"Unit name: {Name}");
            fullDescribedStatus.AppendLine($"Unit team: {UnitIdentifier.TeamId}");
            fullDescribedStatus.Append(UnitStats.ToShortString());

            return fullDescribedStatus.ToString();
        }
    }
}