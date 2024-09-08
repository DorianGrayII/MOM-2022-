using MHUtils;
using MOM;

public class TransportRequest
{
    public Group cargo;

    public Group transport;

    public Vector3i loadingPosition;

    public bool IsValid()
    {
        if (this.cargo == null || !this.cargo.alive || this.transport == null || !this.transport.alive || this.cargo.GetPlane() != this.transport.GetPlane())
        {
            return false;
        }
        return true;
    }

    public void Initialize()
    {
        if (this.IsValid())
        {
            AIGroupDesignation designation = this.transport.GetDesignation();
            if (designation.designation != AIGroupDesignation.Designation.Transfer)
            {
                designation.NewDesignation(AIGroupDesignation.Designation.Transfer, new Destination(this.loadingPosition, this.transport.GetPlane().arcanusType, aggressive: false));
            }
            if (designation.destinationPosition == null)
            {
                designation.destinationPosition = new Destination(this.loadingPosition, this.transport.GetPlane().arcanusType, aggressive: false);
            }
        }
    }
}
