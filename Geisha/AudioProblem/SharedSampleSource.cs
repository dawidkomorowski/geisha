using CSCore;

namespace AudioProblem
{
    public class SharedSampleSource : SampleAggregatorBase, ISampleSource
    {
        public SharedSampleSource(ISampleSource source) : base(source)
        {
        }
    }
}