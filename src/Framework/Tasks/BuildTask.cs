﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;

using static DeluxeJournal.Tasks.TaskParameterAttribute;

namespace DeluxeJournal.Framework.Tasks
{
    internal class BuildTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter(TaskParameterNames.Building, TaskParameterTag.Building)]
            public string? BuildingType { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Building;

            public override bool EnableSmartIconCount => BuildingType != "Farmhouse";

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is BuildTask buildTask)
                {
                    BuildingType = buildTask.BuildingType;
                    Count = buildTask.MaxCount;
                }
            }

            public override ITask? Create(string name)
            {
                return BuildingType != null ? new BuildTask(name, BuildingType, Count) : null;
            }
        }

        /// <summary>The building type. An internal name that is used as the key in <see cref="Game1.buildingData"/>.</summary>
        public string BuildingType { get; set; }

        /// <summary>Serialization constructor.</summary>
        public BuildTask() : base(TaskTypes.Build)
        {
            BuildingType = string.Empty;
        }

        public BuildTask(string name, string buildingType, int count) : base(TaskTypes.Build, name)
        {
            BuildingType = buildingType;
            MaxCount = count;

            if (Game1.buildingData.TryGetValue(buildingType, out var data))
            {
                BasePrice = data.BuildCost;
            }
        }

        public override bool ShouldShowProgress()
        {
            return MaxCount > 1;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            if (BuildingType == "Farmhouse")
            {
                events.ModEvents.GameLoop.UpdateTicked += OnUpdateTicked;
            }
            else
            {
                events.BuildingConstructed += OnBuildingConstructed;
            }
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ModEvents.GameLoop.UpdateTicked -= OnUpdateTicked;
            events.BuildingConstructed -= OnBuildingConstructed;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (CanUpdate() && Game1.player.daysUntilHouseUpgrade.Value >= 3)
            {
                MarkAsCompleted();
            }
        }

        private void OnBuildingConstructed(object? sender, BuildingConstructedEventArgs e)
        {
            if (CanUpdate() && e.NameAfterConstruction == BuildingType)
            {
                IncrementCount();
            }
        }
    }
}
