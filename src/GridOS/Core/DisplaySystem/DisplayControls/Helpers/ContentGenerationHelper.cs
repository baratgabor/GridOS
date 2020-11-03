namespace IngameScript
{
    struct ContentGenerationHelper
    {
        public int RemainingLineCapacity { get; set; }
        public IWordWrapper WordWrapper { get; }

        public ContentGenerationHelper(int remainingLineCapacity, IWordWrapper wordWrapper)
        {
            RemainingLineCapacity = remainingLineCapacity;
            WordWrapper = wordWrapper;
        }
    }
}
