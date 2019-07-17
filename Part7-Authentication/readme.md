# Facial Login using Custom Vision Model

## 1. Why Custom Vision?

Install:
https://www.npmjs.com/package/react-responsive-modal

`npm install react-webcam`    
`npm install @types/react-webcam`   
`npm i react-responsive-modal`

If it shows error : "The key 'abc' is not sorted alphabetically", add the following rule inside tslint.json file, under "rules":
"object-literal-sort-keys": false


Add Camera: in App.tsx
//  import Webcam
import * as Webcam from "react-webcam";

// import Modal
import Modal from 'react-responsive-modal';

// add new state to App.tsx states interface.

interface IState {
    updateVideoList: any,
    player: any,
    playingURL: string
    videoList: object,
    refCamera: any
}

//  set the states in the constructor

class App extends React.Component<{}, IState>{
    public constructor(props: any) {
        super(props);
        this.state = {
            player: null,
            playingURL: "",
            updateVideoList: null,
            videoList: [],
            refCamera: React.createRef(),
        }
    }

// Add webcam to the webapp

public render() {
        return (<div>

            <div>
                <Webcam
                    audio={false}
                    screenshotFormat="image/jpeg"
                    ref={this.state.refCamera}
                />
            </div>

Run and see the webcam.
If it shows error : "The key 'authenticated' is not sorted alphabetically tsconfig", add the following rule inside tslint.json file, under "rules":
"object-literal-sort-keys": false
