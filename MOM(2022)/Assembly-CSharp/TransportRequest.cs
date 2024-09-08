using MHUtils;
using MOM;
using System;

public class TransportRequest
{
    public Group cargo;
    public Group transport;
    public Vector3i loadingPosition;

    public void Initialize()
    {
        if (this.IsValid())
        {
            AIGroupDesignation designation = this.transport.GetDesignation(true);
            if (designation.designation != AIGroupDesignation.Designation.Transfer)
            {
                designation.NewDesignation(AIGroupDesignation.Designation.Transfer, new Destination(this.loadingPosition, this.transport.GetPlane().arcanusType, false));
            }
            if (designation.destinationPosition == null)
                designation.destinationPosition = new Destination(this.loadingPosition, this.transport.GetPlane().arcanusType, false);
        }
    }

    public bool IsValid()
    {
        return ((this.cargo != null) && (this.cargo.alive && ((this.transport != null) && (this.transport.alive && ReferenceEquals(this.cargo.GetPlane(), this.transport.GetPlane())))));
    }
}

