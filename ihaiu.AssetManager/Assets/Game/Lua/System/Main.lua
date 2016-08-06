UpdateBeat = Event("Update", true)
LateUpdateBeat = Event("LateUpdate", true)
CoUpdateBeat = Event("CoUpdate", true)
FixedUpdateBeat = Event("FixedUpdate", true)

function TestCo2()
	local go = GameObject.Find("/Cube")
	local render = go.renderer
	local mat = render.material
	local i = 1
	
	while true do
		mat.color = Color.red
		coroutine.wait(0.2)		
		mat.color = Color.white
		coroutine.wait(0.2)		
	end
	
	print("coroutine2 over:", i)
end


function TestCo()
	print("******a simple coroutine test")
	print("******current time:"..Time.time)
	coroutine.wait(10)
	print("+++++******sleep time:"..Time.time)
	print("+++++current frame:"..Time.frameCount)
  coroutine.wait(5)
  print("+++++******sleep time:"..Time.time)
  print("+++++current frame:"..Time.frameCount)
	coroutine.step()
	print("+++++******yield frame:"..Time.frameCount)		
	print("+++++******coroutine over")
end


function Main()
  print = Debugger.Log
--  print("~~~~~Main.Main" )
--  print(str_table(_G, "_G"))
  if jit then
    print("<color=blue>!!!!!!!!jit.version_num =" .. tostring(jit.version_num) .. "  jit.status()=" .. tostring(jit.status()) .. "</color>")
  else
    print("<color=blue>jit=nil</color>")
  end
	Time:Init()
	
	--测试协同
--	coroutine.start(TestCo)
	--coroutine.start(TestCo2)
end

function Update(deltatime, unscaledDeltaTime)
  --print("~~~~~Main.Update")
	Time:SetDeltaTime(deltatime, unscaledDeltaTime)
	UpdateBeat()
end

FrameUtils = {}
FrameUtils.list = {}
function FrameUtils:Add(ele)
	table.insert(self.list, ele)
end

function FrameUtils:RunOnce()
	for k,v in pairs(self.list) do
		v()
	end
	-- self.list = {}
end

function OnFFF()
	FrameUtils:RunOnce()
end

function LateUpdate()
--  print("~~~~~Main.LateUpdate")
	LateUpdateBeat()
	CoUpdateBeat()
end

function FixedUpdate(fixedTime)
--  print("~~~~~Main.FixedUpdate")
	Time:SetFixedDelta(fixedTime)
	FixedUpdateBeat()
end

function OnLevelWasLoaded(level)
--  print("~~~~~Main.OnLevelWasLoaded")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationPause(isPause)
	if isPause then
	else
		EnterProto.C_SyncChestInfo_0x301()
	end
end

function OnApplicationFocus(focusStatus)
end