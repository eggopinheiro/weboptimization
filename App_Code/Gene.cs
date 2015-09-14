using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for Gene
/// </summary>
public class Gene : ICloneable, IEquatable<Gene>
{
    private double mTrainId = 0.0;
    private DateTime mTime = DateTime.MinValue;
    private Int16 mLocation = -1;
    private string mUD = "";
    private Int16 mDirection = 0;
    private Int16 mTrack = 0;
    private int mCoordinate;
    private int mStart = Int32.MinValue;
    private int mEnd = Int32.MinValue;
    private string mTrainName = "";
    private StopLocation mStopLocation = null;
    private DateTime mDepartureTime = DateTime.MinValue;
    private DateTime mHeadWayTime = DateTime.MinValue;
    private DateTime mDateRef = DateTime.MinValue;
    private int mValue = 0;
    private double mSpeed = 0.0;
    private double mValueWeight = 1.0;

    private const double COEF = 3.0;

    public void ReloadValue(DateTime pRef)
    {
        mValue = (int)Math.Ceiling(ValueWeight * Math.Pow((pRef - mDepartureTime).TotalHours, COEF));
    }

    public static bool operator ==(Gene obj1, Gene obj2)
    {
        bool lvRes = false;

        if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
        {
            return true;
        }

        if (ReferenceEquals(obj1, null))
        {
            return false;
        }

        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        if ((obj1.mTrainId == obj2.mTrainId) && (obj1.mStopLocation == obj2.mStopLocation))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public static bool operator !=(Gene obj1, Gene obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(Gene other)
    {
        bool lvRes = false;

        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if ((this.mStopLocation != null) || (other.StopLocation != null))
        {
            if ((this.mTrainId == other.mTrainId) && (this.mStopLocation == other.mStopLocation))
            {
                lvRes = true;
            }
        }
        else if ((this.mTrainId == other.mTrainId) && (this.Location == other.Location) && (this.mUD.Equals(other.mUD)))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;

        if (obj is Gene)
        {
            lvRes = Equals(obj as Gene);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int lvHashCode = 0;

            if (mTrainId > 0.0)
            {
                lvHashCode = mTrainId.GetHashCode();
                if (mStopLocation != null)
                {
                    lvHashCode = (lvHashCode * 397) ^ mStopLocation.GetHashCode();
                }
            }

            return lvHashCode;
        }
    }

    public DateTime HeadWayTime
    {
        get { return mHeadWayTime; }
        set { mHeadWayTime = value; }
    }

    public double Speed
    {
        get { return mSpeed; }
        set { mSpeed = value; }
    }

    public double ValueWeight
    {
        get { return mValueWeight; }
        set { mValueWeight = value; }
    }

    public int Value
    {
        get 
        { 
            return mValue; 
        }
        set
        {
            mValue = value;
        }
    }

    public string TrainName
    {
        get { return mTrainName; }
        set { mTrainName = value; }
    }

    public DateTime Time
    {
        get { return mTime; }
        set { mTime = value; }
    }

    public Int16 Location
    {
        get { return mLocation; }
        set { mLocation = value; }
    }

    public string UD
    {
        get { return mUD; }
        set { mUD = value; }
    }

    public Int16 Direction
    {
        get { return mDirection; }
        set { mDirection = value; }
    }

    public Int16 Track
    {
        get { return mTrack; }
        set { mTrack = value; }
    }

    public int Coordinate
    {
        get { return mCoordinate; }
        set { mCoordinate = value; }
    }

    public double TrainId
    {
        get { return mTrainId; }
        set { mTrainId = value; }
    }

    public int Start
    {
        get { return mStart; }
        set { mStart = value; }
    }

    public int End
    {
        get { return mEnd; }
        set { mEnd = value; }
    }

    public DateTime DepartureTime
    {
        get 
        { 
            return mDepartureTime; 
        }
        set 
        { 
            mDepartureTime = value;
            if (mDepartureTime > DateTime.Now)
            {
                mValue = 1;
            }
            else
            {
                mValue = (int)Math.Ceiling(ValueWeight * Math.Pow((mDateRef - mDepartureTime).TotalHours, COEF));
            }
        }
    }

    public StopLocation StopLocation
    {
        get { return mStopLocation; }
        set { mStopLocation = value; }
    }

	public Gene(DateTime pRefDate)
	{
        mDateRef = pRefDate;
	}

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public override string ToString()
    {
        return "Gene: " + mTrainId + " - " + mTrainName + ", Location: " + mLocation + ", UD: " + mUD + ", Direction: " + mDirection + ", Track: " + mTrack + ", Time: " + mTime;
    }
}