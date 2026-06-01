import {useAuthContext} from "../contexts/AuthContext.tsx";

type Props = {
    title: string;
    description: string;
};

const DashboardHeader = ({title, description}: Props) => {
    const {userInfo} = useAuthContext();

    return (
        <div className="bg-white border-b border-gray-200 px-6 py-4">
            <div className="max-w-7xl mx-auto flex items-center justify-between">
                <div>
                    <h1 className="text-lg font-semibold text-gray-900">{title}</h1>
                    <p className="text-sm text-gray-500">{description}</p>
                </div>
                {userInfo && (
                    <div className="flex items-center gap-3">
                        <img
                            src={userInfo.picture}
                            alt={userInfo.name}
                            className="size-8 rounded-full"
                        />
                        <span className="text-sm text-gray-700">{userInfo.name}</span>
                    </div>
                )}
            </div>
        </div>
    );
};

export default DashboardHeader;
