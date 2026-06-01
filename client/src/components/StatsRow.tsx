import {Card, CardContent} from "@/components/ui/card";

type Props = {
    total: number;
    showing: number;
    potentials: number;
};

const StatsRow = ({total, showing, potentials}: Props) => (
    <div className="grid grid-cols-3 gap-4">
        <Card size="sm">
            <CardContent className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Total DAs</span>
                <span className="text-2xl font-semibold">{total}</span>
            </CardContent>
        </Card>
        <Card size="sm">
            <CardContent className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Showing</span>
                <span className="text-2xl font-semibold">{showing}</span>
            </CardContent>
        </Card>
        <Card size="sm">
            <CardContent className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Potential cold calls</span>
                <span className="text-2xl font-semibold text-yellow-600">{potentials}</span>
            </CardContent>
        </Card>
    </div>
);

export default StatsRow;
