namespace Sudoku.ParticleSwarmOptimization.TSPPSO {
    public class ResultManager
    {
        #region Public Properties

        public int BestDistanceFound { get; private set; }

        public int BestPossibleDistance { get; private set; }

        public int CorrectResultFound { get; private set; }

        public int HighestDistanceFound { get; private set; }

        public double TotalError { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Reset(int bestDistance)
        {
            this.BestDistanceFound = 0;
            this.HighestDistanceFound = 0;
            this.CorrectResultFound = 0;
            this.TotalError = 0;
            this.BestPossibleDistance = bestDistance;
        }

        public void UpdateResults(int distance)
        {
            if (this.BestDistanceFound == 0 || this.BestDistanceFound > distance)
            {
                this.BestDistanceFound = distance;
            }
            if (distance == this.BestPossibleDistance)
            {
                this.CorrectResultFound++;
            }
            if (distance > this.HighestDistanceFound)
            {
                this.HighestDistanceFound = distance;
            }
            this.TotalError += distance;
        }

        #endregion
    }
}
