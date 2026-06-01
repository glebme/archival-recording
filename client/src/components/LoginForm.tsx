import {Card, CardContent, CardDescription, CardHeader, CardTitle} from "@/components/ui/card"
import {Button} from "@/components/ui/button"
import {useGoogleLogin} from "@react-oauth/google";
import {useState} from "react";
import axios from "axios";
import {useNavigate} from "react-router-dom";
import {useAuthContext, type UserInfo as ContextUserInfo} from "../contexts/AuthContext.tsx";

const LoginForm = () => {
    const navigate = useNavigate();
    const {setUser} = useAuthContext();
    const [hasLoginFailed, setHasLoginFailed] = useState<boolean>(false);

    const login = useGoogleLogin({
        onSuccess: async ({access_token}) => {
            try {
                const response = await axios.post<ContextUserInfo>("/auth/google", {accessToken: access_token});
                setUser(response.data);
                navigate("/dashboard");
            } catch (error) {
                console.error(error);
                setHasLoginFailed(true);
            }
        },
        onError: () => setHasLoginFailed(true),
    });

    return (
        <Card className="w-2/5">
            <CardHeader>
                <CardTitle>Sign in</CardTitle>
                <CardDescription>Use your google account to verify and sign in</CardDescription>
            </CardHeader>
            <CardContent className="flex flex-col items-center gap-4">
                <Button
                    className="w-full"
                    variant={"outline"}
                    onClick={() => login()}
                >
                    Continue with Google
                </Button>
                {hasLoginFailed && (
                    <div className="flex flex-col items-center gap-1">
                        <p className="text-red-600">Google login failed</p>
                        <p className="text-red-600">Please try again</p>
                    </div>
                )}
            </CardContent>
        </Card>
    )
}

export default LoginForm
