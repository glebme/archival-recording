export type DAStatus = "Under Assessment" | "Approved" | "Pending" | "Refused";

export type DevelopmentApplication = {
    id: string;
    daNumber: string;
    council: string;
    address: string;
    description: string;
    lodgedDate: string;
    status: DAStatus;
    estimatedCost: string;
};
