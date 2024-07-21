using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LocalSearchEngine.ExtensionMethods;

namespace LocalSearchEngine.Core
{
    public class Searcher
    {
        private readonly List<Regex> nots = new List<Regex>();
        private readonly List<List<Regex>> ands = new List<List<Regex>>();
        private readonly HashSet<string> highlightedWords = new HashSet<string>();

        public Searcher(string initialTerm, bool isClosedBegin, bool isClosedEnd)
        {
            this.And(initialTerm, isClosedBegin, isClosedEnd);
        }

        public IReadOnlyCollection<string> HighlightedWords => this.highlightedWords;

        public void And(string term, bool isClosedBegin, bool isClosedEnd)
        {
            term = term.ToLower();
            term = term.Trim();

            this.ands.Add(new List<Regex>());

            var regex = this.CreateRegex(term, isClosedBegin, isClosedEnd);
            this.ands[this.ands.Count - 1].Add(regex);
            highlightedWords.Add(term);
        }

        public void Or(string term, bool isClosedBegin, bool isClosedEnd)
        {
            term = term.ToLower();
            term = term.Trim();

            var regex = this.CreateRegex(term, isClosedBegin, isClosedEnd);
            this.ands[this.ands.Count - 1].Add(regex);
            highlightedWords.Add(term);
        }

        public void Not(string term, bool isClosedBegin, bool isClosedEnd)
        {
            term = term.ToLower();
            term = term.Trim();

            var regex = this.CreateRegex(term, isClosedBegin, isClosedEnd);
            this.nots.Add(regex);
        }

        public IEnumerable<SearchResult> Search(IndexReader indexReader)
        {
            string doc;
            while ((doc = indexReader.ReadLine()) != null)
            {
                var conditions = this.ands.Select(x => x.ToList()).ToList();
                var positions = conditions.Select(x => new HashSet<int>()).ToList();
                var titleDistance = int.MaxValue - 1;
                var docDistance = int.MaxValue - 1;

                var notMatch = nots.Any(x => x.IsMatch(doc));
                if (notMatch)
                {
                    yield return new SearchResult
                    {
                        Success = false,
                        DocumentId = indexReader.CurrentId
                    };

                    continue;
                }

                var toRemoved = new List<List<Regex>>();
                var titlePositions = new List<HashSet<int>>();
                var title = doc.Substring(0, doc.IndexOf(Constants.TitleDelimiter));
                for (var i = 0; i < conditions.Count; i++)
                {
                    var ors = conditions[i];
                    var set = new HashSet<int>();
                    foreach (var or in ors)
                    {
                        // var foundPositions = title.AllIndexesOf(or, StringComparison.OrdinalIgnoreCase);
                        var foundPositions = or.Matches(title).Cast<Match>().Select(x => x.Index);
                        foreach (var foundPosition in foundPositions)
                        {
                            set.Add(foundPosition);
                        }
                    }

                    if (set.Count > 0)
                    {
                        titlePositions.Add(set);
                        toRemoved.Add(conditions[i]);
                    }
                }

                titleDistance = this.FindSmallestPairwiseDistance(titlePositions).SmallestDifference;
                toRemoved.ForEach(x => conditions.Remove(x));

                toRemoved = new List<List<Regex>>();
                var docPositions = new List<HashSet<int>>();
                for (var i = 0; i < conditions.Count; i++)
                {
                    var ors = conditions[i];
                    var set = new HashSet<int>();
                    foreach (var or in ors)
                    {
                        // var foundPositions = title.AllIndexesOf(or, StringComparison.OrdinalIgnoreCase);
                        var foundPositions = or.Matches(doc).Cast<Match>().Select(x => x.Index);
                        foreach (var foundPosition in foundPositions)
                        {
                            set.Add(foundPosition);
                        }
                    }

                    if (set.Count > 0)
                    {
                        titlePositions.Add(set);
                        toRemoved.Add(conditions[i]);
                    }
                }

                toRemoved.ForEach(x => conditions.Remove(x));
                var docResult = this.FindSmallestPairwiseDistance(titlePositions);
                var excerpt = string.Empty;
                if (docResult.Found)
                {
                    docDistance = docResult.SmallestDifference;

                    var positions2 = new SortedSet<int> { docResult.Pos1, docResult.Pos2 };
                    var segments = this.GetSegments(positions2, doc.Length - 1);
                    var excerptCount = 0;
                    var isEnd = false;
                    foreach (var segment in segments)
                    {
                        if (segment.Start > 0)
                        {
                            excerpt += " ... ";
                        }

                        excerpt += doc.Substring(segment.Start, segment.Length);
                        if (segment.End >= doc.Length - 1)
                        {
                            isEnd = true;
                        }

                        excerptCount++;
                        if (excerptCount > 4)
                        {
                            break;
                        }
                    }

                    if (!isEnd)
                    {
                        excerpt += " ... ";
                    }
                }

                yield return new SearchResult
                {
                    Success = conditions.Count == 0,
                    DocumentId = indexReader.CurrentId,
                    Excerpt = excerpt,
                    Relevance = -Math.Min(titleDistance, docDistance),
                    Title = title
                };
            }
        }

        private List<Segment> GetSegments(IEnumerable<int> positions, int max)
        {
            var segments = new List<Segment>();
            var positionSet = new SortedSet<int>(positions);

            foreach (var position in positionSet)
            {
                var start = Math.Max(0, position - 40);
                var end = Math.Min(max, position + 40);
                if (segments.Count == 0)
                {
                    segments.Add(new Segment(start, end));
                }
                else
                {
                    var lastSegment = segments[segments.Count - 1];
                    var newSegment = new Segment(start, end);
                    if (lastSegment.IsOverlapping(newSegment))
                    {
                        segments[segments.Count - 1] = new Segment(lastSegment, newSegment);
                    }
                    else
                    {
                        segments.Add(newSegment);
                    }
                }
            }

            return segments;
        }

        private int IndexOf(string s, string sub)
        {
            return s.IndexOf(sub, StringComparison.OrdinalIgnoreCase);
        }

        private Regex CreateRegex(string term, bool isClosedBegin, bool isClosedEnd)
        {
            var regexString = term;

            if (isClosedBegin)
            {
                regexString = "\\b" + regexString;
            }

            if (isClosedEnd)
            {
                regexString = regexString + "\\b";
            }

            return new Regex(regexString, RegexOptions.IgnoreCase);
        }

        private FindSmallestPairwiseDistanceResult FindSmallestPairwiseDistance(List<HashSet<int>> positionGroups)
        {
            var result = new FindSmallestPairwiseDistanceResult();
            for (var i=0;i<positionGroups.Count-1;i++)
            {
                for (var j=i+1;j<positionGroups.Count;j++)
                {
                    var group1 = positionGroups[i];
                    var group2 = positionGroups[j];
                    foreach (var pos1 in group1)
                    {
                        foreach (var pos2 in group2)
                        {
                            var difference = Math.Abs(pos1 - pos2);
                            if (difference < result.SmallestDifference)
                            {
                                result.Found = true;
                                result.SmallestDifference = difference;
                                result.Pos1 = Math.Min(pos1, pos2);
                                result.Pos2 = Math.Max(pos1, pos2);
                            }
                        }
                    }
                }
            }

            if (!result.Found)
            {
                if (positionGroups.Count > 0)
                {
                    result.Found = true;
                    result.Pos1 = positionGroups[0].Min();
                    result.Pos2 = result.Pos1;
                }
            }

            return result;
        }

        private class FindSmallestPairwiseDistanceResult
        {
            public int SmallestDifference { get; set; } = int.MaxValue - 1;
            public bool Found { get; set; }
            public int Pos1 { get; set; } = -1;
            public int Pos2 { get; set; } = -1;
        }

        private class Segment
        {
            public Segment(int start, int end)
            {
                this.Start = start;
                this.End = end;
            }

            public Segment(Segment a, Segment b)
            {
                this.Start = Math.Min(a.Start, b.Start);
                this.End = Math.Max(a.End, b.End);
            }

            public int Start { get; }
            public int End { get; }

            public int Length => End - Start + 1;

            public bool IsOverlapping(Segment other)
            {
                if (other.End < this.Start)
                {
                    return false;
                }
                else if (other.End <= this.End)
                {
                    return true;
                }
                else // other.End > this.End
                {
                    return other.Start <= this.End;
                }
            }
        }
    }
}
