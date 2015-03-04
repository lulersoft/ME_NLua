
--加载模型测试

local loadmod={}

local this

function loadmod.Start()
	this=loadmod.this	

	local name="Cube"
	this:LoadBundle(name,loadmod.onLoadComplete)
end


function loadmod.onLoadComplete(uri,bundle)

	Debug.Log(uri)

	if uri=="JX01YX01BRll02" then

		local prefab =bundle.mainAsset -- bundle:Load("JX01YX01BRll02")
		

		local guiGo=GameObject.Instantiate(prefab)
		guiGo.name="JX01YX01BRll02"

		guiGo.transform.localScale = Vector3.one
        guiGo.transform.localPosition = Vector3(1,1,1)
	end

	if uri=="Cube" then

		local prefab =bundle.mainAsset -- bundle:Load("JX01YX01BRll02")		

		local guiGo=GameObject.Instantiate(prefab)
		guiGo.name="Cube"

		guiGo.transform.localScale = Vector3.one
        guiGo.transform.localPosition = Vector3(0,1,1)

        local name="JX01YX01BRll02"
	    this:LoadBundle(name,loadmod.onLoadComplete)
	end

end

return loadmod