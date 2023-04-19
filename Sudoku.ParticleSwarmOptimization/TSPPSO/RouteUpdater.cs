using System;
using System.Linq;
using System.Collections;

namespace Sudoku.ParticleSwarmOptimization.TSPPSO
{

    public class RouteUpdater
    {
        #region Constants and Fields

        public BitArray AvailabilityMask { get; set; }

        private int[] destinationIndex;

        private int itineryLength;

        public bool Isinitialised { get; private set; }

        public BitArray SelectedMask { get; set; }

        private int destinationIndexPointer;

        #endregion


        public void Initialise()
        {
            this.itineryLength = 81;
            this.AvailabilityMask = new BitArray(this.itineryLength, true);
            this.SelectedMask = new BitArray(this.itineryLength, false);
            this.destinationIndex = new int[this.itineryLength];
            this.destinationIndexPointer = 0;
            Isinitialised = true;
        }
        public void AddSection(int pointer, Route section, bool isReversed)
        {
            int startPos = pointer;

            if (isReversed)
            {
                //reverse the order as it is equally valid in reverse
                Array.Reverse(section.Cells);
            }

            this.SetSelectedMask(startPos, section);
            this.SetDestinationIndex(startPos, section);
        }

        //This is called using the currentRoute to add any missing cities to the destinationIndex
        public int[] FinalizeDestinationIndex(Route route)
        {
            this.SelectedMask.SetAll(true);
            this.SelectedMask.And(this.AvailabilityMask);
            foreach (int c in route.Cells.Where(c => this.SelectedMask[c]))
            {
                this.destinationIndex[this.destinationIndexPointer] = c;
                this.destinationIndexPointer++;
            }
            this.Reset();
            return this.GetDestinationIndex();
        }

        public int[] GetDestinationIndex()
        {
            var copy = new int[this.itineryLength];
            this.destinationIndex.CopyTo(copy, 0);
            return copy;
        }
        public void Reset()
        {
            this.destinationIndexPointer = 0;
            this.AvailabilityMask.SetAll(true);
            this.SelectedMask.SetAll(false);
        }
        //Updates the  new route by adding cities,sequentially from the route section
        //providing the cities are not already present
        public void SetDestinationIndex(int startPosition, Route section)
        {


            int p = startPosition;
            for (int i = 0; i < section.SegmentSize; i++)
            {
                if (this.SelectedMask[section.Cells[p]])
                {
                    this.destinationIndex[this.destinationIndexPointer] = section.Cells[p];
                    this.destinationIndexPointer++;
                }
                p++;
                if (p == section.Cells.Length)
                {
                    p = 0;
                }
            }
            //update the City AvailabilityMask
            //sets bits that represent cities that have been included to false
            this.AvailabilityMask.Xor(this.SelectedMask);
        }

        //sets the selected BitArray mask so that
        //only cities that have not been added already are available
        //pointer is set to the start of the segment
        public void SetSelectedMask(int pointer, Route section)
        {
            int p = pointer;
            this.SelectedMask.SetAll(false);
            //foreach city in the section set the appropriate bit
            // in the selected mask
            for (int i = 0; i < section.SegmentSize; i++)
            {
                //set bit to signify that city is to be added if not already used
                this.SelectedMask[section.Cells[p]] = true;

                p++;
                //p is a circular pointer in that it moves from the end of the route 
                // to the start
                if (p == section.Cells.Length)
                {
                    p = 0;
                }
            }
            //in the AvailabilityMask, true=available, false= already used
            //remove cities from the SelectedMask  that have already been added
            this.SelectedMask.And(this.AvailabilityMask);
        }
    }
}
