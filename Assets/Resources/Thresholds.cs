using System;
using System.Collections.Generic;
using System.Linq;

public enum ThresholdCross {
    FromDown, FromUp,
}

public class Thresholds
{
    private List<float> thresholds;
    private int lastCrossedIndex;
    private ThresholdCross lastCrossedDir;

    public Thresholds(List<float> initialThresholds, float current)
    {
        bool isSortedAscending = initialThresholds.SequenceEqual(initialThresholds.OrderBy(x => x));
        if (!isSortedAscending)
        {
            throw new ArgumentException("Thresholds must be sorted in ascending order.");
        }

        thresholds = initialThresholds;
        findThresholdIndex(current); // Initialize the last threshold crossed index to -1 (no threshold crossed)
    }

    // Indexer to access the thresholds list
    public float this[int i]
    {
        get
        {
            return thresholds[i];
        }
    }

    public Dictionary<int, ThresholdCross> CheckThresholdsCrossed(float current)
    {
        var crossedThresholds = new Dictionary<int, ThresholdCross>();
        var crossed = CheckThresholdCrossed(current);
        if (crossed == null) return null;

        // Check if any thresholds are crossed and trigger the event if so
        while (crossed != null) {
            crossedThresholds.Add(crossed.Value.i, crossed.Value.dir);
            crossed = CheckThresholdCrossed(current);
        }
        return crossedThresholds;
    }

    private (int i, ThresholdCross dir)? CheckThresholdCrossed(float current)
    {
        // maybe check if current is in range of i thresholds for up and down both
        // crossing from down
        if (lastCrossedDir == ThresholdCross.FromDown && current < thresholds[lastCrossedIndex])
        {
            lastCrossedDir = ThresholdCross.FromUp;
            return (lastCrossedIndex, lastCrossedDir);
        }

        if (lastCrossedDir == ThresholdCross.FromDown && thresholds.Count < lastCrossedIndex + 1 && current >= thresholds[lastCrossedIndex+1])
        {
            lastCrossedIndex++;
            lastCrossedDir = ThresholdCross.FromDown;
            return (lastCrossedIndex, lastCrossedDir);
        }

        if (lastCrossedDir == ThresholdCross.FromUp && current >= thresholds[lastCrossedIndex])
        {
            lastCrossedDir = ThresholdCross.FromDown;
            return (lastCrossedIndex, lastCrossedDir);
        }

        if (lastCrossedDir == ThresholdCross.FromUp && lastCrossedIndex > 0 && current < thresholds[lastCrossedIndex])
        {
            lastCrossedIndex--;
            lastCrossedDir = ThresholdCross.FromUp;
            return (lastCrossedIndex, lastCrossedDir);
        }
        return null;
    }
    private void findThresholdIndex(float current)
    {
        // Initialize the last threshold crossed index to -1 (no threshold crossed)
        // Check if any thresholds are crossed and trigger the event if so
        for (int i = 0; i < thresholds.Count; i++)
        {
            if (current >= thresholds[i])
            {
                lastCrossedIndex = i;
                lastCrossedDir = ThresholdCross.FromDown;
            }
            else if (i == 0) // handle if value is below first threshold
            {
                lastCrossedIndex = i;
                lastCrossedDir = ThresholdCross.FromUp;
            }
        }
    }
}