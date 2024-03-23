import Link from "next/link";
import SignInForm from "./signin-form";

export default function Page() {
    return (
        <div>
            <div className="w-full pt-36">
                <SignInForm />
                <div className="text-center mt-5">
                    Don&apos;t have an account?&nbsp;
                    <Link href="/signup" className="text-blue-500 hover:text-blue-800">
                        Sign up here.
                    </Link>
                </div>
            </div>
        </div>
    )
}