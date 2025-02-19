﻿using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class CollectTask : ItemTaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = ObjectIdsConstraint)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            [TaskParameter(TaskParameterNames.Quality, TaskParameterTag.Quality, Parent = TaskParameterNames.Item, Constraints = Constraint.GE0)]
            public int Quality { get; set; } = 0;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item;

            public override bool EnableSmartIconCount => true;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is CollectTask collectTask)
                {
                    ItemIds = collectTask.ItemIds;
                    Count = collectTask.MaxCount;
                    Quality = collectTask.Quality;
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new CollectTask(name, ItemIds, Count, Quality) : null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public CollectTask() : base(TaskTypes.Collect)
        {
        }

        public CollectTask(string name, IList<string> itemIds, int count, int quality)
            : base(TaskTypes.Collect, name, itemIds, count, quality)
        {
            if (itemIds.Count == 0 || ItemRegistry.GetDataOrErrorItem(itemIds.First()).Category != SObject.ringCategory)
            {
                MaxCount = count;
            }
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ItemCollected += OnItemCollected;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ItemCollected -= OnItemCollected;
        }

        private void OnItemCollected(object? sender, ItemReceivedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && CheckItemMatch(e.Item))
            {
                IncrementCount(e.Count);
            }
        }
    }
}
