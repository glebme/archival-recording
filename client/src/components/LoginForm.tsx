import {Card, CardContent, CardDescription, CardHeader, CardTitle} from "@/components/ui/card"
import {Button} from "@/components/ui/button.tsx";
import {type CodeResponse, useGoogleLogin} from "@react-oauth/google";
import {useState} from "react";
import axios from "axios";
import {useNavigate} from "react-router-dom";

const LoginForm = () => {
    const navigate = useNavigate();
    const [hasLoginFailed, setHasLoginFailed] = useState<boolean>(false);

    const login = useGoogleLogin({
        onSuccess: async (codeResponse: CodeResponse) => {
            await axios.post("/api/auth/google", {code: codeResponse.code})
            navigate("/dashboard");
        },
        onError: () => setHasLoginFailed(true),
        flow: 'auth-code'
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