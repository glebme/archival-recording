import {Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle} from "@/components/ui/card";
import {Button} from "@/components/ui/button";
import {Building2, Calendar, DollarSign, MapPin, Search, Star} from "lucide-react";
import type {DAStatus, DevelopmentApplication} from "../types/da.ts";

const STATUS_STYLES: Record<DAStatus, string> = {
    "Under Assessment": "bg-yellow-100 text-yellow-800",
    Approved: "bg-green-100 text-green-800",
    Pending: "bg-blue-100 text-blue-800",
    Refused: "bg-red-100 text-red-800",
};

type Props = {
    applications: DevelopmentApplication[];
    potentials: Set<string>;
    onTogglePotential: (id: string) => void;
};

const DAList = ({applications, potentials, onTogglePotential}: Props) => {
    if (applications.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center py-24 text-muted-foreground">
                <Search className="size-10 mb-3 opacity-30"/>
                <p className="text-sm">No development applications match your filters.</p>
            </div>
        );
    }

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {applications.map((da) => {
                const isPotential = potentials.has(da.id);
                return (
                    <Card key={da.id} className={isPotential ? "ring-2 ring-yellow-400" : ""}>
                        <CardHeader className="border-b pb-3">
                            <div className="flex items-start justify-between gap-2">
                                <div>
                                    <CardTitle className="font-mono text-sm">{da.daNumber}</CardTitle>
                                    <CardDescription className="mt-1">{da.council}</CardDescription>
                                </div>
                                <span
                                    className={`shrink-0 rounded-full px-2 py-0.5 text-xs font-medium ${STATUS_STYLES[da.status]}`}>
                                    {da.status}
                                </span>
                            </div>
                        </CardHeader>
                        <CardContent className="space-y-2 pt-3">
                            <div className="flex items-start gap-1.5 text-xs text-muted-foreground">
                                <MapPin className="size-3.5 mt-0.5 shrink-0"/>
                                <span>{da.address}</span>
                            </div>
                            <div className="flex items-start gap-1.5 text-xs text-muted-foreground">
                                <Building2 className="size-3.5 mt-0.5 shrink-0"/>
                                <span className="line-clamp-2">{da.description}</span>
                            </div>
                            <div className="flex items-center justify-between text-xs text-muted-foreground">
                                <div className="flex items-center gap-1.5">
                                    <Calendar className="size-3.5"/>
                                    <span>
                                        {new Date(da.lodgedDate).toLocaleDateString("en-AU", {
                                            day: "numeric",
                                            month: "short",
                                            year: "numeric",
                                        })}
                                    </span>
                                </div>
                                <div className="flex items-center gap-1">
                                    <DollarSign className="size-3.5"/>
                                    <span className="font-medium text-foreground">{da.estimatedCost}</span>
                                </div>
                            </div>
                        </CardContent>
                        <CardFooter className="justify-between gap-2">
                            <span className="text-xs text-muted-foreground">
                                {isPotential ? "Marked as potential" : "Mark as potential cold call"}
                            </span>
                            <Button
                                size="icon-sm"
                                variant={isPotential ? "default" : "outline"}
                                onClick={() => onTogglePotential(da.id)}
                                title={isPotential ? "Remove from potentials" : "Mark as potential cold call"}
                            >
                                <Star className={isPotential ? "fill-current" : ""}/>
                            </Button>
                        </CardFooter>
                    </Card>
                );
            })}
        </div>
    );
};

export default DAList;
