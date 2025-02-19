﻿using StardewModdingAPI;
using StardewValley;
using DeluxeJournal.Events;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class CraftTask : TaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = Constraint.Craftable | Constraint.NotEmpty)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item;

            public override bool EnableSmartIconCount => true;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is CraftTask craftTask)
                {
                    ItemIds = craftTask.ItemIds;
                    Count = craftTask.MaxCount;
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new CraftTask(name, ItemIds, Count) : null;
            }
        }

        /// <summary>The qualified item IDs of the items to be crafted.</summary>
        public IList<string> ItemIds { get; set; }

        /// <summary>Serialization constructor.</summary>
        public CraftTask() : base(TaskTypes.Craft)
        {
            ItemIds = Array.Empty<string>();
        }

        public CraftTask(string name, IList<string> itemIds, int count) : base(TaskTypes.Craft, name)
        {
            ItemIds = itemIds;

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
            events.ItemCrafted += OnItemCrafted;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ItemCrafted -= OnItemCrafted;
        }

        private void OnItemCrafted(object? sender, ItemReceivedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && ItemIds.Contains(e.Item.QualifiedItemId))
            {
                IncrementCount(e.Count);
            }
        }
    }
}
