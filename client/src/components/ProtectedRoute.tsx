import {Navigate, Outlet} from "react-router-dom";
import {useAuthContext} from "../contexts/AuthContext.tsx";

const ProtectedRoute = () => {
    const {userInfo, authLoading} = useAuthContext();

    if (authLoading) {
        return (
            <div className="min-h-screen flex justify-center items-center bg-gray-100">
                <p className="text-gray-500">Loading...</p>
            </div>
        );
    }

    return userInfo ? <Outlet/> : <Navigate to="/login" replace/>;
};

export default ProtectedRoute;
