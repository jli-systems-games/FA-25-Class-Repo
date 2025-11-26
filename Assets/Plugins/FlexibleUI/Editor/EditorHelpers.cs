using System;
using System.Linq;
using UnityEngine;

namespace JeffGrawAssets.FlexibleUI
{
public static class EditorHelpers
{
    public enum Alignment { Left, Right, Center, Justified }
    public enum FlexibleSpaceAllocation { Proportional, LargestMinSizeFirst, SmallestFlexibleAreaFirst }

    public static Rect[] DivideRect(Alignment alignment, Rect totalRect, float minElementPadding, float maxElementPadding, float edgePadding, params (float labelWidth, float minFieldWidth, float maxFieldWidth)[] sections)
        => DivideRect(alignment, FlexibleSpaceAllocation.Proportional, totalRect, minElementPadding, maxElementPadding, edgePadding, sections);

    public static Rect[] DivideRect(Alignment alignment, FlexibleSpaceAllocation flexibleSpaceAllocation, Rect totalRect, float minElementPadding, float maxElementPadding, float edgePadding, params (float labelWidth, float minFieldWidth, float maxFieldWidth)[] sections)
    {
        var n = sections.Length;
        if (n == 0) 
            return Array.Empty<Rect>();

        if (alignment == Alignment.Justified)
        {
            alignment = Alignment.Center;
            maxElementPadding = float.MaxValue;
        }

        int nonEmptySections = 0;
        for (int i = 0; i < n; i++)
            if (sections[i] != default)
                nonEmptySections++;

        float totalLabel = minElementPadding * (nonEmptySections - 1), totalMinField = 0f, totalMaxField = 0f;
        for (int i = 0; i < n; i++)
        {
            totalLabel    += sections[i].labelWidth;
            totalMinField += sections[i].minFieldWidth;
            totalMaxField += sections[i].maxFieldWidth;
        }

        var contentWidth = totalRect.width - 2 * edgePadding;
        var rects = new Rect[n];
        var totalSectionWidth = 0f;

        if (flexibleSpaceAllocation == FlexibleSpaceAllocation.Proportional)
        {
            var minWidth = totalLabel + totalMinField;
            var maxWidth = totalLabel + totalMaxField;
            var growthPotential = totalMaxField - totalMinField;
            var targetWidth = Mathf.Clamp(contentWidth, minWidth, maxWidth);
            var growthFactor = growthPotential > 0 ? (targetWidth - minWidth) / growthPotential : 0f;
            for (int i = 0; i < n; i++)
            {
                var growth = growthFactor * (sections[i].maxFieldWidth - sections[i].minFieldWidth);
                rects[i].width = sections[i].labelWidth + sections[i].minFieldWidth + growth;
                totalSectionWidth += rects[i].width;
            }
        }
        else
        {
            var indices = flexibleSpaceAllocation == FlexibleSpaceAllocation.LargestMinSizeFirst
                ? Enumerable.Range(0, n).OrderByDescending(i => sections[i].minFieldWidth).ToArray()
                : Enumerable.Range(0, n).OrderBy(i => sections[i].maxFieldWidth - sections[i].minFieldWidth).ToArray();

            var remainingWidth = Mathf.Max(totalRect.width - totalLabel - totalMinField, 0);
            for (int i = 0; i < n; i++)
            {
                var idx = indices[i];
                var growth = Mathf.Min(sections[idx].maxFieldWidth - sections[idx].minFieldWidth, remainingWidth);
                rects[idx].width = sections[idx].labelWidth + sections[idx].minFieldWidth + growth;
                totalSectionWidth += rects[idx].width;
                remainingWidth -= growth;
            }
        }

        var availablePaddingSpace = contentWidth - totalSectionWidth;
        var desiredPadding = nonEmptySections > 1 ? availablePaddingSpace / (nonEmptySections - 1) : 0f;
        var actualPadding = Mathf.Clamp(desiredPadding, minElementPadding, maxElementPadding);
        var totalWidth = totalSectionWidth + (nonEmptySections - 1) * actualPadding;

        var startX = alignment switch
        {
            Alignment.Left  => totalRect.x + edgePadding,
            Alignment.Right => totalRect.x + totalRect.width - edgePadding - totalWidth,
            _               => totalRect.x + edgePadding + (contentWidth - totalWidth) / 2,
        };

        for (int i = 0; i < n; i++)
        {
            var width = rects[i].width;
            rects[i] = new Rect(startX, totalRect.y, width, totalRect.height);
            startX += i < n - 1 && sections[i] != default ? width + actualPadding : width;
        }

        return rects;
    }
}
}