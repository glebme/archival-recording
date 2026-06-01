import {Input} from "@/components/ui/input";
import {Button} from "@/components/ui/button";
import {Search, Star} from "lucide-react";

type Props = {
    search: string;
    onSearchChange: (value: string) => void;
    councilFilter: string;
    onCouncilFilterChange: (value: string) => void;
    councils: string[];
    showPotentialsOnly: boolean;
    onShowPotentialsOnlyChange: (value: boolean) => void;
};

const DAFilters = ({
                       search,
                       onSearchChange,
                       councilFilter,
                       onCouncilFilterChange,
                       councils,
                       showPotentialsOnly,
                       onShowPotentialsOnlyChange,
                   }: Props) => (
    <div className="flex gap-3 flex-wrap">
        <div className="relative flex-1 min-w-60">
            <Search
                className="absolute left-2.5 top-1/2 -translate-y-1/2 size-4 text-muted-foreground pointer-events-none"/>
            <Input
                className="pl-8"
                placeholder="Search by DA number, address or description…"
                value={search}
                onChange={(e) => onSearchChange(e.target.value)}
            />
        </div>
        <select
            className="h-8 rounded-lg border border-input bg-transparent px-2.5 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 min-w-52"
            value={councilFilter}
            onChange={(e) => onCouncilFilterChange(e.target.value)}
        >
            <option value="all">All councils</option>
            {councils.map((c) => (
                <option key={c} value={c}>{c}</option>
            ))}
        </select>
        <Button
            variant={showPotentialsOnly ? "default" : "outline"}
            onClick={() => onShowPotentialsOnlyChange(!showPotentialsOnly)}
            className="gap-1.5"
        >
            <Star className={showPotentialsOnly ? "fill-current" : ""}/>
            Potentials only
        </Button>
    </div>
);

export default DAFilters;
