import {useMemo, useState} from "react";
import DashboardHeader from "../components/DashboardHeader.tsx";
import StatsRow from "../components/StatsRow.tsx";
import DAFilters from "../components/DAFilters.tsx";
import DAList from "../components/DAList.tsx";
import type {DevelopmentApplication} from "../types/da.ts";

const MOCK_DAS: DevelopmentApplication[] = [
    {
        id: "1",
        daNumber: "DA/2024/0412",
        council: "Blacktown City Council",
        address: "14 Flushcombe Rd, Blacktown NSW 2148",
        description: "Demolition of existing structures and construction of a 4-storey mixed-use development",
        lodgedDate: "2024-03-15",
        status: "Under Assessment",
        estimatedCost: "$2,400,000",
    },
    {
        id: "2",
        daNumber: "DA/2024/0187",
        council: "City of Parramatta Council",
        address: "88 Church St, Parramatta NSW 2150",
        description: "Alterations and additions to heritage-listed commercial building, including new facade works",
        lodgedDate: "2024-01-22",
        status: "Pending",
        estimatedCost: "$850,000",
    },
    {
        id: "3",
        daNumber: "DA/2024/0553",
        council: "Cumberland Council",
        address: "231 Merrylands Rd, Merrylands NSW 2160",
        description: "Construction of a 7-storey residential flat building with 42 units and basement parking",
        lodgedDate: "2024-05-08",
        status: "Under Assessment",
        estimatedCost: "$9,100,000",
    },
    {
        id: "4",
        daNumber: "DA/2023/1142",
        council: "The Hills Shire Council",
        address: "3 Solent Circuit, Norwest NSW 2153",
        description: "Demolition of existing office building and construction of a new 3-storey commercial building",
        lodgedDate: "2023-11-30",
        status: "Approved",
        estimatedCost: "$4,200,000",
    },
    {
        id: "5",
        daNumber: "DA/2024/0298",
        council: "Canterbury-Bankstown Council",
        address: "156 Canterbury Rd, Canterbury NSW 2193",
        description: "Change of use from industrial to creative arts hub with internal fit-out works",
        lodgedDate: "2024-02-14",
        status: "Approved",
        estimatedCost: "$320,000",
    },
    {
        id: "6",
        daNumber: "DA/2024/0674",
        council: "Georges River Council",
        address: "45 Montgomery St, Kogarah NSW 2217",
        description: "Demolition of existing warehouse and construction of an 8-storey mixed-use tower",
        lodgedDate: "2024-06-01",
        status: "Pending",
        estimatedCost: "$14,500,000",
    },
    {
        id: "7",
        daNumber: "DA/2024/0101",
        council: "Blacktown City Council",
        address: "72 Main St, Blacktown NSW 2148",
        description: "Partial demolition and refurbishment of existing 2-storey commercial premises",
        lodgedDate: "2024-01-05",
        status: "Refused",
        estimatedCost: "$650,000",
    },
    {
        id: "8",
        daNumber: "DA/2024/0488",
        council: "City of Parramatta Council",
        address: "19 Fennell St, North Parramatta NSW 2151",
        description: "Construction of a 12-storey build-to-rent residential tower with ground floor retail",
        lodgedDate: "2024-04-17",
        status: "Under Assessment",
        estimatedCost: "$22,000,000",
    },
    {
        id: "9",
        daNumber: "DA/2023/0987",
        council: "The Hills Shire Council",
        address: "9 Century Circuit, Baulkham Hills NSW 2153",
        description: "Demolition of existing structures and construction of a childcare centre for 90 children",
        lodgedDate: "2023-09-12",
        status: "Approved",
        estimatedCost: "$1,800,000",
    },
    {
        id: "10",
        daNumber: "DA/2024/0362",
        council: "Cumberland Council",
        address: "88 Auburn Rd, Auburn NSW 2144",
        description: "Construction of a 5-storey mixed-use development with retail at ground level and 24 units above",
        lodgedDate: "2024-03-28",
        status: "Pending",
        estimatedCost: "$5,600,000",
    },
    {
        id: "11",
        daNumber: "DA/2024/0519",
        council: "Canterbury-Bankstown Council",
        address: "310 Hume Hwy, Bankstown NSW 2200",
        description: "Demolition of existing service station and construction of a 6-storey mixed-use building",
        lodgedDate: "2024-05-22",
        status: "Under Assessment",
        estimatedCost: "$8,750,000",
    },
    {
        id: "12",
        daNumber: "DA/2023/1204",
        council: "Georges River Council",
        address: "2 MacMahon St, Hurstville NSW 2220",
        description: "Alterations and additions to existing shopping centre including new rooftop dining precinct",
        lodgedDate: "2023-12-19",
        status: "Approved",
        estimatedCost: "$3,100,000",
    },
];

const COUNCILS = [...new Set(MOCK_DAS.map((da) => da.council))].sort();

const DashboardPage = () => {
    const [search, setSearch] = useState("");
    const [councilFilter, setCouncilFilter] = useState("all");
    const [showPotentialsOnly, setShowPotentialsOnly] = useState(false);
    const [potentials, setPotentials] = useState<Set<string>>(new Set());

    const togglePotential = (id: string) => {
        setPotentials((prev) => {
            const next = new Set(prev);
            next.has(id) ? next.delete(id) : next.add(id);
            return next;
        });
    };

    const filtered = useMemo(() => {
        const q = search.toLowerCase();
        return MOCK_DAS.filter((da) => {
            const matchesSearch =
                q === "" ||
                da.daNumber.toLowerCase().includes(q) ||
                da.address.toLowerCase().includes(q) ||
                da.description.toLowerCase().includes(q);
            const matchesCouncil =
                councilFilter === "all" || da.council === councilFilter;
            const matchesPotential = !showPotentialsOnly || potentials.has(da.id);
            return matchesSearch && matchesCouncil && matchesPotential;
        });
    }, [search, councilFilter, showPotentialsOnly, potentials]);

    return (
        <div className="min-h-screen bg-gray-50">
            <DashboardHeader
                title="Development Applications"
                description="Search and manage DA leads for archival recording"
            />
            <div className="max-w-7xl mx-auto px-6 py-6 space-y-6">
                <StatsRow
                    total={MOCK_DAS.length}
                    showing={filtered.length}
                    potentials={potentials.size}
                />
                <DAFilters
                    search={search}
                    onSearchChange={setSearch}
                    councilFilter={councilFilter}
                    onCouncilFilterChange={setCouncilFilter}
                    councils={COUNCILS}
                    showPotentialsOnly={showPotentialsOnly}
                    onShowPotentialsOnlyChange={setShowPotentialsOnly}
                />
                <DAList
                    applications={filtered}
                    potentials={potentials}
                    onTogglePotential={togglePotential}
                />
            </div>
        </div>
    );
};

export default DashboardPage;
